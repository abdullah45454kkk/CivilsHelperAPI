using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using DataAccess.EmailServices.IEmailService;

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
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Civils Assistance", "mohamdenabdallah444@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using var client = new SmtpClient();
            try
            {
                // Connect with STARTTLS for port 587
                Console.WriteLine("Connecting to smtp-relay.brevo.com...");
                await client.ConnectAsync(_configuration["Email:Smtp:Host"],
                          int.Parse(_configuration["Email:Smtp:Port"]),
                          MailKit.Security.SecureSocketOptions.StartTls);

                // Authenticate
                Console.WriteLine("Authenticating...");
                await client.AuthenticateAsync(_configuration["Email:Smtp:Username"],
                                             _configuration["Email:Smtp:Password"]);

                // Send
                Console.WriteLine("Sending email...");
                await client.SendAsync(emailMessage);
                Console.WriteLine("Email sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.ToString());
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}