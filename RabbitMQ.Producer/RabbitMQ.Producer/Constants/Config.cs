using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Constants
{
    public static class Config
    {
        public const string RabbitHostName = "rabbit";
        public const string RabbitPort = "9091";
        public const string RabbitUsername = "admin";
        public const string RabbitPassword = "admin";
        public const string RabbitExchangeName = "retry-exchange";
        public const string RabbitQueueName = "mail-queue";
    }
}
