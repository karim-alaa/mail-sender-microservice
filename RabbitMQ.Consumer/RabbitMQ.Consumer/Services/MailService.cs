using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using RabbitMQ.Consumer.Dtos;
using RabbitMQ.Consumer.Dtos.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Services
{

    public interface IMailService
    {
        Task<bool> SendMail(EmailDto emailDto);
        Task LogStuckMail(string messageBody);
    }

    public class MailService : IMailService
    {
        private readonly AppConfig _config;
        public MailService(AppConfig config)
        {
            _config = config;
        }

        public async Task<bool> SendMail(EmailDto emailDto)
        {
            var message = new MimeMessage
            {
                Subject = emailDto.Subject,
                Body = new TextPart(TextFormat.Text)
                {
                    Text = emailDto.Body
                }
            };

            if (emailDto.From != _config.Smtp.Username)
                return false;

            // ignore from email right now!
            // TODO: may we need to handle the from email later
            message.From.Add(MailboxAddress.Parse(_config.Smtp.Username));

            foreach(string to in emailDto.To) 
                message.To.Add(MailboxAddress.Parse(to));

            if (emailDto.CC != null)
            {
                foreach (string cc in emailDto.CC)
                    message.Cc.Add(MailboxAddress.Parse(cc));
            }

            if (emailDto.BCC != null)
            {
                foreach (string bcc in emailDto.BCC)
                    message.Bcc.Add(MailboxAddress.Parse(bcc));
            }

            // SMTP Setup
            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(_config.Smtp.Host, _config.Smtp.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config.Smtp.Username, _config.Smtp.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
   
        public async Task LogStuckMail(string messageBody)
        {
            var message = new MimeMessage
            {
                Subject = "Stuck Mail, in RabbitMQ RetryService - Mail Consumer",
                Body = new TextPart(TextFormat.Text)
                {
                    Text = messageBody
                }
            };

            message.From.Add(MailboxAddress.Parse(_config.Smtp.Username));
            message.To.Add(MailboxAddress.Parse(_config.Smtp.Username));

            // SMTP Setup
            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(_config.Smtp.Host, _config.Smtp.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config.Smtp.Username, _config.Smtp.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }
}
