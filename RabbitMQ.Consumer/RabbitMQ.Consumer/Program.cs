using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Consumer.Constants;
using RabbitMQ.Consumer.Listeners;
using RabbitMQ.Consumer.Services;
using System;
using System.Text;

namespace RabbitMQ.Consumer
{
    public class Program
    {
        public static void Main()
        {
            // Setup DI
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IMailService, MailService>()
                .AddSingleton<IListener, MailListener>()
                .BuildServiceProvider();

            // Activate Mail Listener
            var _mailListener = serviceProvider.GetService<IListener>();
            _mailListener.Listen();
        }
    }
}
