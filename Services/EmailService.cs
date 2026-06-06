
using SendGrid;
using SendGrid.Helpers.Mail;

public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string toEmail, string toName, string subject, string body)
    {
        var client = new SendGridClient(_config["SendGrid:ApiKey"]);

        var from = new EmailAddress(
            _config["SendGrid:FromEmail"],
            _config["SendGrid:FromName"]
        );

        var to = new EmailAddress(toEmail, toName);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
        await client.SendEmailAsync(msg);
    }
}