using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Consumer.Constants;
using RabbitMQ.Producer.Constants;
using RabbitMQ.Producer.Data;
using RabbitMQ.Producer.Dtos.Config;
using RabbitMQ.Producer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Queues.RabbitMQ
{
    public interface ICustomQueue
    {
        public IModel GetChannel();
        public IBasicProperties GetBasicProperties();
    }
    public class CustomRabbitMQ
    {
        private readonly AppConfig _config;
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        private IBasicProperties properties;
        private readonly ConcurrentDictionary<ulong, Guid> outstandingConfirms;
        private readonly IServiceProvider _provider;
        public CustomRabbitMQ(AppConfig config, IServiceProvider provider)
        {
            _config = config;
            _provider = provider;
            outstandingConfirms = new();
            CreateNewChannel();
        }

        private IModel CreateNewChannel()
        {
            try
            {
                factory = new()
                {
                    Uri = new Uri($"amqp://{_config.MassTransit.Username}:{_config.MassTransit.Password}@{_config.MassTransit.Host}:{_config.MassTransit.Port}")
                };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.ConfirmSelect();
                properties = channel.CreateBasicProperties();

                channel.BasicAcks += async (sender, ea) =>
                {
                    await HandleBasicAck(ea.DeliveryTag, ea.Multiple);
                };

                channel.BasicNacks += async (sender, ea) =>
                {
                    await HandleBasicNack(ea.DeliveryTag, ea.Multiple);
                };
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            return channel;
        }

        public IModel GetChannel() 
        {
            if (channel.IsOpen)
                return channel;
            return CreateNewChannel();
        }

        public IBasicProperties GetBasicProperties()
        {
            properties.Persistent = true;
            return properties;
        }

        public List<Guid> GetOutstandingConfirm(ulong SequenceNumber, bool IsMultiple)
        {
            if (IsMultiple)
               return outstandingConfirms.Where(m => m.Key <= SequenceNumber).Select(m => m.Value).ToList();
            
            outstandingConfirms.TryGetValue(SequenceNumber, out Guid MessageId);
            return new List<Guid>() { MessageId };
        }

        public bool AddOutstandingConfirm(ulong SequenceNumber, Guid MessageId)
        {
            return outstandingConfirms.TryAdd(SequenceNumber, MessageId);
        }

        public bool RemoveOutstandingConfirm(ulong SequenceNumber, bool IsMultiple)
        {
            if(IsMultiple)
            {
                var toRemove = outstandingConfirms.Keys.Where(m => m <= SequenceNumber).ToArray();
                foreach (var key in toRemove)
                {
                    outstandingConfirms.Remove(key, out _);
                }
                return true;
            }
            return outstandingConfirms.TryRemove(SequenceNumber, out _);
        }

        private async Task HandleBasicAck(ulong DeliveryTag, bool IsMultiple)
        {
            using (var scope = _provider.CreateScope())
            {
                DataContext _context = scope.ServiceProvider.GetRequiredService<DataContext>();

                List<Guid> MessageIds = GetOutstandingConfirm(DeliveryTag, IsMultiple);

                List<Message> messages = await _context.Messages.Where(m => MessageIds.Contains(m.Id)).ToListAsync();
                foreach (Message msg in messages)
                {
                    msg.Status = MessageStatuses.INQUEUE;
                    msg.UpdatedAt = DateTime.Now;
                }

                if (messages.Count >= _config.DBConfig.DeservedBulk)
                    await _context.BulkUpdateAsync(messages, options => options.PropertiesToInclude = new List<string>() { nameof(Message.Status), nameof(Message.UpdatedAt) });
                else
                    _context.Messages.UpdateRange(messages);

                await _context.SaveChangesAsync();
            }
            RemoveOutstandingConfirm(DeliveryTag, IsMultiple);
        }

        private async Task HandleBasicNack(ulong DeliveryTag, bool IsMultiple)
        {
            using (var scope = _provider.CreateScope())
            {
                DataContext _context = scope.ServiceProvider.GetRequiredService<DataContext>();
                IMapper _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                List<Guid> MessageIds = GetOutstandingConfirm(DeliveryTag, IsMultiple);
                List<Message> messages = await _context.Messages.Where(m => MessageIds.Contains(m.Id)).ToListAsync();
                List<StuckMessage> stuckMessages = new();
                foreach (Message msg in messages)
                {
                    msg.NAckesTimes++;

                    // prepare stuck message record
                    StuckMessage stuckMessage = _mapper.Map<StuckMessage>(msg);
                    stuckMessage.StuckReason = MessageStuckReasons.QUEUE_NACK;
                    stuckMessages.Add(stuckMessage);

                    msg.Status = MessageStatuses.LOGGED;
                    msg.UpdatedAt = DateTime.Now;
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    if (messages.Count >= _config.DBConfig.DeservedBulk)
                        await _context.BulkUpdateAsync(messages, options => options.PropertiesToInclude = new List<string>() { nameof(Message.Status), nameof(Message.UpdatedAt) });
                    else
                        _context.Messages.UpdateRange(messages);
                    
                    if (stuckMessages.Count >= _config.DBConfig.DeservedBulk)
                        await _context.BulkInsertAsync(stuckMessages);
                    else
                        _context.Messages.UpdateRange(messages);

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
            }
            RemoveOutstandingConfirm(DeliveryTag, IsMultiple);
        }
    }
}
