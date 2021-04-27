using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Producer.Constants;
using RabbitMQ.Producer.Dtos;
using RabbitMQ.Producer.Dtos.Config;
using RabbitMQ.Producer.Models;
using RabbitMQ.Producer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailController : Controller
    {
        private readonly IQueueService _queueService;
        private readonly AppConfig _config;

        public MailController(IQueueService queueService, AppConfig config)
        {
            _queueService = queueService;
            _config = config;
        }

        // POST: Mail/Publish
        [HttpPost]
        [Route("Publish")]
        public async Task<ActionResult> Create([FromBody] MessageDto messageDto)
        {
            try
            {
                Message message = new()
                {
                    Id = messageDto.Id,
                    Body = JsonConvert.SerializeObject(messageDto.EmailData).ToString(),
                    ExchangeName = _config.MassTransit.Exchange,
                    RoutingKey = ExchangeRoutingKeys.MAIL
                };
                if(await _queueService.ScheduleMessage(message))
                    return Ok(Messages.MESSAGE_SENT);
                return BadRequest(Messages.MESSAGE_ALREADY_SENT);
            }
            catch(Exception)
            {
                return BadRequest(Messages.SOME_THING_WENT_WRONG);
            }
        }
    }
}
