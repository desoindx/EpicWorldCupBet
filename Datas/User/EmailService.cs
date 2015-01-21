using System;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

public class EmailService : IIdentityMessageService
{
    public Task SendAsync(IdentityMessage message)
    {
        return SendMessage(message.Subject, message.Body, message.Destination);
    }

    private static Task SendMessage(string subject, string body, string to)
    {
        // Compose a message
        var mail = new MailMessage(ConfigurationManager.AppSettings["MAILGUN_SMTP_LOGIN"], to)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        // Send it!
        var client = new SmtpClient
        {
            Port = Int32.Parse(ConfigurationManager.AppSettings["MAILGUN_SMTP_PORT"]),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MAILGUN_SMTP_LOGIN"],
                ConfigurationManager.AppSettings["MAILGUN_SMTP_PASSWORD"]),
            Host = ConfigurationManager.AppSettings["MAILGUN_SMTP_SERVER"]
        };

        return client.SendMailAsync(mail);
    }
}
