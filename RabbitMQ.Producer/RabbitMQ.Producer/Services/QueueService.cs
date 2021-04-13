using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Producer.Dtos;
using RabbitMQ.Producer.Dtos.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Services
{
    public interface IQueueService
    {
        public void PublishMessage(QueueDto queueDto);
    }

    public class QueueService : IQueueService
    {

        private readonly AppConfig _config;
        public QueueService(AppConfig config)
        {
            _config = config;
        }

        public void PublishMessage(QueueDto queueDto)
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
            channel.BasicPublish(queueDto.ExchangeName, queueDto.RoutingKey, null, queueDto.Body);
        }
    }
}
