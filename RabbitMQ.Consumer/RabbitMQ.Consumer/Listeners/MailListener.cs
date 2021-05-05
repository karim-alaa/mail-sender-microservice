using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Consumer.Constants;
using RabbitMQ.Consumer.Data;
using RabbitMQ.Consumer.Dtos;
using RabbitMQ.Consumer.Dtos.Config;
using RabbitMQ.Consumer.Models;
using RabbitMQ.Consumer.Services;
using System;
using System.Text;

namespace RabbitMQ.Consumer.Listeners
{
    public class MailListener : IListener
    {
        private readonly IMailService _mailService;
        private readonly AppConfig _config;
        private readonly IServiceProvider _provider;

        public MailListener(IMailService mailService, AppConfig config, IServiceProvider provider)
        {
            _mailService = mailService;
            _config = config;
            _provider = provider;
        }

        public void Listen()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri($"amqp://{_config.MassTransit.Username}:{_config.MassTransit.Password}@{_config.MassTransit.Host}:{_config.MassTransit.Port}"),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(_config.MassTransit.NetworkRecoveryIntervalSeconds)
            };

            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();
           
            // Consume 2 by 2 messages
            channel.BasicQos(0, 2, false);

            EventingBasicConsumer consumer = new(channel);

            consumer.Received += async (sender, ea) =>
            {
                try
                {
                    #region check retries
                    if (ea.BasicProperties.Headers != null)
                    {
                        ea.BasicProperties.Headers.TryGetValue("x-delivery-count", out object deliveryCountObj);
                        int deliveryCount = Convert.ToInt32(deliveryCountObj);
                        if (deliveryCount > _config.MaxMessageRetries)
                        {
                            // Save in database in case you need to requeue
                            string emailRequestMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
                            Message messageRequest = JsonConvert.DeserializeObject<Message>(emailRequestMessage);

                            using (var scope = _provider.CreateScope())
                            {
                                var _context = scope.ServiceProvider.GetService<DataContext>();
                                try
                                {
                                    // add the stuck email request
                                    messageRequest.Status = MessageStatuses.ERROR;
                                    messageRequest.UpdatedAt = DateTime.Now;
                                    _context.Messages.Update(messageRequest);
                                    await _context.SaveChangesAsync();

                                    Console.WriteLine("message is stuck, notification send to database");
                                }
                                catch (Exception)
                                {
                                    // Record may be delete
                                    _context.Entry(messageRequest).State = EntityState.Detached;

                                    await _context.Messages.AddAsync(messageRequest);
                                    await _context.SaveChangesAsync();

                                    Console.WriteLine("record removed from database, a new record created");
                                }
                            }
                            
                        // remove from queue
                        channel.BasicReject(ea.DeliveryTag, false);

                        throw new Exception(message: "message is stuck, removed from queue");
                        }
                    }
                    #endregion
                    
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Message messageRequestDone = JsonConvert.DeserializeObject<Message>(message);
                    EmailRequestDto emailRequestDto = JsonConvert.DeserializeObject<EmailRequestDto>(messageRequestDone.Body);

                    Console.WriteLine(messageRequestDone.Body);

                    using (var scope = _provider.CreateScope())
                    {
                        var _context = scope.ServiceProvider.GetService<DataContext>();
                        try
                        {
                            bool messageSent = await _mailService.SendMail(emailRequestDto);
                            if (messageSent)
                            {
                                Console.WriteLine("Mail Sent!");

                                channel.BasicAck(ea.DeliveryTag, false);
                                messageRequestDone.Status = MessageStatuses.PROCEED;
                                messageRequestDone.UpdatedAt = DateTime.Now;
                                _context.Messages.Update(messageRequestDone);
                                await _context.SaveChangesAsync();
                                Console.WriteLine("message status updated to proceed");
                            }
                            else
                            {
                                // requeue
                                channel.BasicNack(ea.DeliveryTag, false, true);
                            }
                        }
                        catch (Exception)
                        {
                            // Record may be delete, or exchange server down
                        }
                    }

                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error: {ex.Message} /n");
                }
            };

            channel.BasicConsume(_config.MassTransit.Queue, false, consumer);

            Console.ReadLine();
        }
    }
}
