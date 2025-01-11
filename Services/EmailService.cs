using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CharityProject.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_configuration["MailSettings:SMTPServer"])
                {
                    Port = int.Parse(_configuration["MailSettings:Port"]),
                    Credentials = new NetworkCredential(_configuration["MailSettings:Username"], _configuration["MailSettings:Password"]),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["MailSettings:FromEmail"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Handle error (consider logging)
                return false;
            }
        }
    }
}
