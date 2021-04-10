using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Dtos
{
    public class EmailDto
    {
        [Required(ErrorMessage ="Subject is Required")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Body is Required")]
        public string Body { get; set; }
        [Required(ErrorMessage = "From email is Required")]
        public string From { get; set; }
        [Required(ErrorMessage = "To emails is Required")]
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
    }
}
