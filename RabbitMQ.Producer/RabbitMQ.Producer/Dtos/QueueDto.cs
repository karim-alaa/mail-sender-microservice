using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Dtos
{
    public class QueueDto
    {
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
        public byte[] Body { get; set; }
    }
}
