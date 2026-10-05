using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        public EmailSender(IOptions<EmailSettings> options) => _settings = options.Value;

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            using var message = new MailMessage(_settings.From, to, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}
