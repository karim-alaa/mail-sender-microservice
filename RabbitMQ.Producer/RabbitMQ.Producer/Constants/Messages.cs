using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Constants
{
    public static class Messages
    {
        public const string MESSAGE_SENT = "MESSAGE_SENT";
        public const string MESSAGE_ALREADY_SENT = "MESSAGE_ALREADY_SENT";
        public const string SOME_THING_WENT_WRONG = "SOME_THING_WENT_WRONG";
    }
}
