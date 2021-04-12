using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Dtos.Config
{
    public class MassTransitOptions
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
