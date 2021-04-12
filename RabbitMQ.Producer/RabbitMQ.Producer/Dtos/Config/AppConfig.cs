using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Dtos.Config
{
    public class AppConfig
    {
        public SmtpOptions Smtp { get; set; }
        public MassTransitOptions MassTransit { get; set; }
        public string ConnectionString { get; set; }
    }
}
