using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Consumer.Constants;
using RabbitMQ.Producer.Dtos;
using RabbitMQ.Producer.Dtos.Config;
using RabbitMQ.Producer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Services
{
    public interface IReDeliveryService
    {
        Task ReDeliverStuckMessage(Data.DataContext _context);
    }

    public class ReDeliveryService : IReDeliveryService
    {

        private readonly AppConfig _config;
        private readonly IQueueService _queueService;
        private readonly IMapper _mapper;
         
        public ReDeliveryService(AppConfig config, IQueueService queueService, IMapper mapper)
        {
            _config = config;
            _queueService = queueService;
            _mapper = mapper;
        }

        public async Task ReDeliverStuckMessage(Data.DataContext _context)
        {
            DateTime DeservedTimeForError = DateTime.Now.AddMinutes(-_config.ReDelivery.MaxTotalMinuteForError);
            DateTime DeservedTimeForInQueue = DateTime.Now.AddMinutes(-_config.ReDelivery.MaxTotalMinuteForInQueue);

            List<Message> stuckMessages = await _context.Messages.Where(m => 
            (m.Status == MessageStatuses.ERROR && m.UpdatedAt <= DeservedTimeForError) || 
            (m.Status == MessageStatuses.INQUEUE && m.UpdatedAt <= DeservedTimeForInQueue))
             .ToListAsync();

            foreach(Message message in stuckMessages)
            {
                // stuck message, log it
                if (message.ReDeliveryTimes >= _config.ReDelivery.MaxRetryTimes)
                {
                    using var transaction = _context.Database.BeginTransaction();

                    StuckMessage stuckMessage = _mapper.Map<StuckMessage>(message);
                    await _context.StuckMessages.AddAsync(stuckMessage);

                    message.Status = MessageStatuses.LOGGED;
                    _context.Messages.Update(message);

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                else
                    await _queueService.ReScheduleMessage(message);
            }
        }
    }
}
