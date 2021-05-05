using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Consumer.Constants;
using RabbitMQ.Producer.Dtos;
using RabbitMQ.Producer.Dtos.Config;
using RabbitMQ.Producer.Models;
using RabbitMQ.Producer.Queues.RabbitMQ;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Services
{
    public interface IQueueService
    {
        Task<bool> ScheduleMessage(Message message);
        Task<bool> ReScheduleMessage(Message message);
    }

    public class QueueService : IQueueService
    {
        private readonly Data.DataContext _context;
        private readonly CustomRabbitMQ _customRabbitMQ;
        

        public QueueService(Data.DataContext context, CustomRabbitMQ customRabbitMQ)
        {
            _context = context;
            _customRabbitMQ = customRabbitMQ;
        }

        public async Task<bool> ScheduleMessage(Message message)
        {
            try
            {
                message.Status = MessageStatuses.INPRODUCER;
                message.UpdatedAt = DateTime.Now;

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                // publish the message
                PublishMessage(message);
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 2601))
            {
                return false;
            }
        }

        public async Task<bool> ReScheduleMessage(Message message)
        {
            message.Status = MessageStatuses.INPRODUCER;
            message.UpdatedAt = DateTime.Now;
            message.ReDeliveryTimes++;
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            try
            {
                // publish the message
                PublishMessage(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void PublishMessage(Message message)
        {
            // It is undesirable to keep many TCP connections open at the same time
            IModel channel = _customRabbitMQ.GetChannel();
            IBasicProperties properties = _customRabbitMQ.GetBasicProperties();
            ulong sequenceNumber = channel.NextPublishSeqNo;
          
            _customRabbitMQ.AddOutstandingConfirm(sequenceNumber, message.Id);
            channel.BasicPublish(message.ExchangeName, message.RoutingKey, properties, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
            Console.WriteLine("Message Published");
        }
    }
}
