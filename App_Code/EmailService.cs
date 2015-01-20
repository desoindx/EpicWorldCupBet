using System;
using System.Configuration;
using System.Net.Mail;
using RestSharp;

public static class EmailService
{
    public static void SendSimpleMessage()
    {
        // Compose a message
        var mail = new MailMessage("NoReply@EpicBet.com", "xavier.desoindre@hotmail.fr")
        {
            Subject = "Hello",
            Body = "Testing some Mailgun awesomness"
        };

        // Send it!
        var client = new SmtpClient();
        client.Port = Int32.Parse(ConfigurationManager.AppSettings["MAILGUN_SMTP_PORT"]);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MAILGUN_SMTP_LOGIN"],
            ConfigurationManager.AppSettings["MAILGUN_SMTP_PASSWORD"]);
        client.Host = ConfigurationManager.AppSettings["MAILGUN_SMTP_SERVER"];

        client.Send(mail);
    }
}
