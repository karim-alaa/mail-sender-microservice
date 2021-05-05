using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Constants
{
    public static class MessageStuckReasons
    {
        public const string MAX_RETRY_EXCEED = "MAX_RETRY_EXCEED";
        public const string QUEUE_NACK = "QUEUE_NACK";
    }
}
