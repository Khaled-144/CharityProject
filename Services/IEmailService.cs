namespace CharityProject.Services
{
    public interface IEmailService
    {
        bool SendEmail(string toEmail, string subject, string body);
    }
}
