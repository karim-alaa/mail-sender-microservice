using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Producer.Constants;
using RabbitMQ.Producer.Dtos;
using RabbitMQ.Producer.Dtos.Config;
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
        public ActionResult Create([FromBody] EmailDto emailDto)
        {
            try
            {
                QueueDto queueDto = new()
                {
                    Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(emailDto)),
                    ExchangeName = _config.MassTransit.Exchange,
                    RoutingKey = ExchangeRoutingKeys.MAIL
                };
                _queueService.PublishMessage(queueDto);
                return Ok("message sent");
            }
            catch
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}
