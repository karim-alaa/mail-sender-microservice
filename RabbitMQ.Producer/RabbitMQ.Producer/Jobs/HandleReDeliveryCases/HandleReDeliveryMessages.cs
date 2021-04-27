using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using RabbitMQ.Producer.Data;
using RabbitMQ.Producer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RechargeAutoAction.Jobs.ManageRechargeCases
{
    [DisallowConcurrentExecution]
    public class HandleReDeliveryMessages : IJob
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<HandleReDeliveryMessages> _logger;
        public HandleReDeliveryMessages(IServiceProvider provider, ILogger<HandleReDeliveryMessages> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _provider.CreateScope())
            {
                // Resolve the Scoped service
                var _context = scope.ServiceProvider.GetService<DataContext>();
                var _reDeliveryService = scope.ServiceProvider.GetService<IReDeliveryService>();
                await HandleRedeliveryCases(_reDeliveryService, _context);
            }

            await Task.CompletedTask;
        }

        private async Task HandleRedeliveryCases(IReDeliveryService _reDeliveryService, DataContext _context)
        {
            try
            {
                await _reDeliveryService.ReDeliverStuckMessage(_context);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            _logger.LogInformation("ReDeliver Stuck Message Case Manager Job Running..." + DateTime.Now.ToString());
        }

    }
}
