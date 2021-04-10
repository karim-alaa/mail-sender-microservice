using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Consumer.Constants
{
    public static class Config
    {
        public const string RabbitHostName = "localhost";
        public const string RabbitPort = "9091";
        public const string RabbitUsername = "admin";
        public const string RabbitPassword = "admin";
        public const string RabbitExchangeName = "retry-exchange";
        public const string RabbitQueueName = "mail-queue";
        public const int MaxRetries = 10;
    }
}
