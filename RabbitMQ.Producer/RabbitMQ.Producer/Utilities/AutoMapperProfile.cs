using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RabbitMQ.Producer.Dtos;
using RabbitMQ.Producer.Models;

namespace RabbitMQ.Producer.Utilities
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Message
            //CreateMap<Message, MessagePublishDto>();
            //CreateMap<MessagePublishDto, Message>();
        }
     }
}
