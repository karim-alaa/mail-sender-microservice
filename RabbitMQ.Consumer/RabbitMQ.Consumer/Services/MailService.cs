using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using RabbitMQ.Consumer.Data;
using RabbitMQ.Consumer.Dtos;
using RabbitMQ.Consumer.Dtos.Config;
using RabbitMQ.Consumer.Models;
using RabbitMQ.Consumer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Services
{

    public interface IMailService
    {
        Task<bool> SendMail(EmailRequestDto emailDto);
    }

    public class MailService : IMailService
    {
        private readonly AppConfig _config;
        private readonly IHelper _helper;
        public MailService(AppConfig config, IHelper helper)
        {
            _config = config;
            _helper = helper;
        }

        public async Task<bool> SendMail(EmailRequestDto emailDto)
        {
            var message = new MimeMessage
            {
                Subject = emailDto.Subject,
            };

            /*
            if (emailDto.From != _config.Smtp.Username)
                return false;
            */

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


            BodyBuilder MailBodyBuilder = new () { TextBody = emailDto.Body };

            // Add Attachments
            List<string> Attachments = new();
            if (emailDto.Attachments != null)
            {
                Random rand = new();
                foreach (string attachmentDataURL in emailDto.Attachments)
                {
                    string FileName = _helper.Base64ToFile(attachmentDataURL, rand);
                    MailBodyBuilder.Attachments.Add(FileName);
                    Attachments.Add(FileName);
                }
            }

            message.Body = MailBodyBuilder.ToMessageBody();

            // SMTP Setup
               
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(_config.Smtp.Host, _config.Smtp.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_config.Smtp.Username, _config.Smtp.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
          
            _helper.RemoveTempFiles(Attachments);
            return true;
        }
    }
}
