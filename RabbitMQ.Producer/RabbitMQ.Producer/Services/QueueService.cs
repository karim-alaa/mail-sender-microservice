using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Producer.Constants;
using RabbitMQ.Producer.Dtos;
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
        public void PublishMessage(QueueDto queueDto)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri($"amqp://{Config.RabbitUsername}:{Config.RabbitPassword}@{Config.RabbitHostName}:{Config.RabbitPort}")
            };
            // TODO: check if you need to keep connection and channel created only one time?
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // use name of queue if you don't have exchanges and bindings by routing key yet!
            //cause we use the default exchange which uses queue name instead of routing key
            channel.BasicPublish(queueDto.ExchangeName, queueDto.RoutingKey, null, queueDto.Body);
        }
    }
}
