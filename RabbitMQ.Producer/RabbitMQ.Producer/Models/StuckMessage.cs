using Newtonsoft.Json;
using RabbitMQ.Producer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Models
{
    public class StuckMessage
    {
        public StuckMessage()
        {
            CreatedAt = DateTime.Now;
            ReDeliveryTimes = 0;
        }

        public Guid Id { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public string StuckReason { get; set; }
        public int ReDeliveryTimes { get; set; }
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
    }
}
