using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Dtos.Config
{
    public sealed class AppConfig
    {
        public SmtpOptions Smtp { get; set; }
        public MassTransitOptions MassTransit { get; set; }
        public int MaxMessageRetries { get; set; }
        public string ConnectionString { get; set; }
    }
}
