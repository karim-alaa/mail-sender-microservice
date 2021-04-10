using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Consumer.Constants;
using RabbitMQ.Consumer.Dtos;
using RabbitMQ.Consumer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Listeners
{
    public class MailListener : IListener
    {
        private readonly IMailService _mailService;
        public MailListener(IMailService mailService)
        {
            _mailService = mailService;
        }
        public void Listen()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri($"amqp://{Config.RabbitUsername}:{Config.RabbitPassword}@{Config.RabbitHostName}:{Config.RabbitPort}")
            };
           
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
           
            channel.BasicQos(0, 3, false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (sender, e) =>
            {
                try
                {
                    
                    #region check retries
                    if (e.BasicProperties.Headers != null)
                    {
                        e.BasicProperties.Headers.TryGetValue("x-delivery-count", out object deliveryCountObj);
                        int deliveryCount = Convert.ToInt32(deliveryCountObj);
                        if (deliveryCount > Config.MaxRetries)
                        {
                            // TODO: save in database in case you need to requeue

                            // send notification email, or replace with a log
                            await _mailService.LogStuckMail(Encoding.UTF8.GetString(e.Body.ToArray()));
                            
                            // remove from queue
                            channel.BasicReject(e.DeliveryTag, false);

                            Console.WriteLine("message is stuck, removed from queue");

                            throw new Exception(message: "logical error occurred!");
                        }
                    }
                    #endregion
                    
                    string message = Encoding.UTF8.GetString(e.Body.ToArray());
                    EmailDto emailDto = JsonConvert.DeserializeObject<EmailDto>(message);
                    Console.WriteLine(message);

                    bool messageSent = await _mailService.SendMail(emailDto);
                    if (messageSent)
                    {
                        channel.BasicAck(e.DeliveryTag, false);
                        Console.WriteLine("Mail Sent!");
                    }
                    else
                    {
                        // requeue
                        channel.BasicNack(e.DeliveryTag, false, true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error: {ex.Message} /n");
                   
                }

                
            };

            channel.BasicConsume(Config.RabbitQueueName, false, consumer);
            Console.ReadLine();
        }
    }
}
