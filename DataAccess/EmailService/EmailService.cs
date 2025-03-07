using DataAccess.EmailServices.IEmailService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient
            {
                Host = _configuration["Email:Smtp:Host"], // "smtp-relay.brevo.com"
                Port = int.Parse(_configuration["Email:Smtp:Port"]), // 587
                EnableSsl = true, // Brevo requires SSL
                Credentials = new NetworkCredential(
                    _configuration["Email:Smtp:Username"], // "875998001@smtp-brevo.com"
                    _configuration["Email:Smtp:Password"]  // "hJE4FtGM3mO0H79L"
                )
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:Smtp:Username"], "Civils Assistance"), // Sender name
                Subject = subject,
                Body = message,
                IsBodyHtml = true // Support for HTML emails
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
