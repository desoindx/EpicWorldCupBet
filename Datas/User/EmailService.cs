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
        var mail = new MailMessage("Support@epicsportexchange.com", to)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        // Send it!
        var client = new SmtpClient
        {
            Credentials = new System.Net.NetworkCredential("Support@epicsportexchange.com","vRa77330"),
            Host = "localhost"
        };

        return client.SendMailAsync(mail);
    }
}
