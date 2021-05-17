using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Consumer.Constants;
using RabbitMQ.Consumer.Data;
using RabbitMQ.Consumer.Dtos.Config;
using RabbitMQ.Consumer.Listeners;
using RabbitMQ.Consumer.Services;
using RabbitMQ.Consumer.Utilities;
using System;
using System.IO;
using System.Text;

namespace RabbitMQ.Consumer
{
    public class Program
    {
        private static IConfiguration _iconfiguration;
        public static void Main()
        {

            // Add appsettings
            GetAppSettingsFile();

            var _config = _iconfiguration.Get<AppConfig>();

            // Setup DI
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddSingleton<IHelper, Helper>()
                .AddSingleton<IMailService, MailService>()
                .AddSingleton<IListener, MailListener>()
                .AddSingleton(_config)
                .AddDbContext<DataContext>(options => options.UseSqlServer(_config.ConnectionString));

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            // Activate Mail Listener
            var _mailListener = serviceProvider.GetService<IListener>();
            _mailListener.Listen();
        }
        
        static void GetAppSettingsFile()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                 .AddJsonFile($"appsettings.{env}.json", true, true);
            _iconfiguration = builder.Build();
        }
        
    }
}
