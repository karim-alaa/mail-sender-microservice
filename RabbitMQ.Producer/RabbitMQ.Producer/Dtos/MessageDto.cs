using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Dtos
{

    public class MessageDto
    {
        [Required(ErrorMessage = "Id is Required")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Email Request is Required")]
        public EmailRequestDto EmailData { get; set; }
    }
}
