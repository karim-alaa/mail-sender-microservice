using Newtonsoft.Json;
using RabbitMQ.Consumer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
