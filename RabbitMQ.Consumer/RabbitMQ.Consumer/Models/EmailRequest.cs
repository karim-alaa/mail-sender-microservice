using Newtonsoft.Json;
using RabbitMQ.Consumer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Models
{
    public class EmailRequest
    {
        public EmailRequest()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromEmail { get; set; }
        public string ToEmails { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public DateTime CtreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void ParseMessage(string emailRequestMessage)
        {
            EmailDto emailRequestDto = JsonConvert.DeserializeObject<EmailDto>(emailRequestMessage);

            Subject = emailRequestDto.Subject;
            Body = emailRequestDto.Body;
            ToEmails = string.Join(",", emailRequestDto.To);

            if(emailRequestDto.From != null)
                FromEmail = emailRequestDto.From;

            if (emailRequestDto.CC != null)
                CC = string.Join(",", emailRequestDto.CC);

            if (emailRequestDto.BCC != null)
                BCC = string.Join(",", emailRequestDto.BCC);

            CtreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
