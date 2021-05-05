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
        public ReDeliveryOptions ReDelivery { get; set; }
        public CustomLoggingOptions CustomLogging { get; set; }
        public DBOptions DBConfig { get; set; }
    }
}
