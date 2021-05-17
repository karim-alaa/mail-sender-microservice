using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Constants
{
    public static class MessageStuckReasons
    {
        public const string MAX_MAIL_SERVER_RETRY = "MAX_MAIL_SERVER_RETRY";
        public const string MAIL_SERVER_ERROR = "MAIL_SERVER_ERROR";
    }
}
