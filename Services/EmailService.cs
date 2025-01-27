using CharityProject.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public bool SendEmail(string toEmail, string subject, string body)
    {
        try
        {
            var smtpClient = new SmtpClient(_configuration["MailSettings:SMTPServer"])
            {
                Port = 25, // Typically used for SSL, but if SSL is not supported, use 25 or check with provider for the correct port
                Credentials = new NetworkCredential(_configuration["MailSettings:Username"], _configuration["MailSettings:Password"]),
                Timeout = 120000 // Increase timeout if needed
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["MailSettings:FromEmail"]),
                Subject = subject,
                IsBodyHtml = true
            };

            // Define the HTML body of the email
            string htmlBody = $@"
<html dir='rtl'>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f9f9f9;
                margin: 0;
                padding: 20px;
            }}
            .email-container {{
                max-width: 600px;
                margin: 0 auto;
                padding: 20px;
                background-color: #ffffff;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            }}
            h1 {{
                color: #333;
                text-align: right; /* Align title to the right */
            }}
            p {{
                font-size: 16px;
                color: #555;
                text-align: right; /* Align text to the right */
            }}
            .verification-code {{
                font-size: 24px;
                font-weight: bold;
                color: #4CAF50;
                margin: 20px 0;
                padding: 10px;
                background-color: #e8f5e9;
                border-radius: 4px;
                text-align: center;
            }}
            .footer {{
                font-size: 12px;
                color: #888;
                text-align: center;
                margin-top: 30px;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <h1>رمز التحقق</h1>
            <p>مرحباً،</p>
            <p>رمز التحقق الخاص بك هو:</p>
            <div class='verification-code'>{body}</div>
            <p>يرجى استخدام هذا الرمز لإتمام عملية إعادة تعيين كلمة المرور.</p>
            <p>إذا لم تقم بطلب هذا، يرجى تجاهل هذا البريد.</p>
            <div class='footer'>
                <p>&copy; {DateTime.Now.Year} جميعة الإسكان التعاوني في المدينة المنورة. جميع الحقوق محفوظة.</p>
            </div>
        </div>
    </body>
</html>
";


            mailMessage.Body = htmlBody;
            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
            return true;
        }
        catch (Exception)
        {
            // Handle exception as needed without logging
            return false;
        }
    }


}
