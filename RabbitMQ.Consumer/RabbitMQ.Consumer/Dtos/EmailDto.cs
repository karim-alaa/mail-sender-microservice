using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Dtos
{
    public class EmailDto
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
    }
}
