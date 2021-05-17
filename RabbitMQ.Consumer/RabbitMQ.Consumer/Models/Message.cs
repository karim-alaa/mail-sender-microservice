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
        public string SequenceNumber { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public MessageErrorLog PrepareMessageError(string ErrorMessage)
        {
            MessageErrorLog MessageErrorLog = new()
            {
                Id = Guid.NewGuid(),
                MessageId = Id,
                ExchangeName = ExchangeName,
                SequenceNumber = SequenceNumber,
                Body = Body,
                Status = Status,
                ErrorMessage = ErrorMessage,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            return MessageErrorLog;
        }
    }
}
