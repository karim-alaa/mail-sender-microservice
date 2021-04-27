using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Consumer.Constants;
using RabbitMQ.Producer.Dtos;
using RabbitMQ.Producer.Dtos.Config;
using RabbitMQ.Producer.Models;
using System;
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

        private readonly AppConfig _config;
        private readonly Data.DataContext _context;
         
        public QueueService(AppConfig config, Data.DataContext context)
        {
            _config = config;
            _context = context;
        }

        public async Task<bool> ScheduleMessage(Message message)
        {
            try
            {
                message.Status = MessageStatuses.INQUEUE;
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
            message.Status = MessageStatuses.INQUEUE;
            message.UpdatedAt = DateTime.Now;
            message.ReDeliveryTimes++;

            try
            {
                // publish the message
                PublishMessage(message);

                _context.Messages.Update(message);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void PublishMessage(Message message)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri($"amqp://{_config.MassTransit.Username}:{_config.MassTransit.Password}@{_config.MassTransit.Host}:{_config.MassTransit.Port}")
            };
            // TODO: check if you need to keep connection and channel created only one time?
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // use name of queue if you don't have exchanges and bindings by routing key!
            // cause we use the default exchange which uses queue name instead of routing key
            channel.BasicPublish(message.ExchangeName, message.RoutingKey, null, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
        }
    }
}
