using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Consumer.Constants
{
    public static class MessageStatuses
    {
        public const string INQUEUE = "INQUEUE";
        public const string ERROR = "ERROR";
        public const string PROCEED = "PROCEED";
    }
}
