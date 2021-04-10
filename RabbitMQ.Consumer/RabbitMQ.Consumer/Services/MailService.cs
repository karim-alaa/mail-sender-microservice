using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using RabbitMQ.Consumer.Constants;
using RabbitMQ.Consumer.Dtos;
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
        public SmtpClient TEClient;
        private static string TEMail => Credentials.MasterEmailAddress;
        private static string TEPassword => Credentials.MasterEmailPassword;
        private static int TEPort => Credentials.MasterEmailPort;
        private static string TEHost => Credentials.MasterEmailHost;

       
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

            if (emailDto.From != TEMail)
                return false;

            // ignore from email right now!
            message.From.Add(MailboxAddress.Parse(TEMail));

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
            await client.ConnectAsync(TEHost, TEPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(TEMail, TEPassword);
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

            message.From.Add(MailboxAddress.Parse(TEMail));

            message.To.Add(MailboxAddress.Parse(TEMail));

            // SMTP Setup
            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(TEHost, TEPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(TEMail, TEPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }
}
