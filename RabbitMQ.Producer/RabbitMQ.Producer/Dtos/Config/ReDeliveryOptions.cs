using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Dtos.Config
{
    public class ReDeliveryOptions
    {
        public double MaxTotalMinuteForError { get; set; }
        public double MaxTotalMinuteForInQueue { get; set; }
        public double MaxRetryTimes { get; set; }
        public string JobRunTime { get; set; }
    }
}
