using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using Datas.Entities;
using Pop3;

/// <summary>
/// Summary description for Foutains
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[ScriptService]
public class Messages : WebService
{
    public Messages()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public List<string> GetMessages(string user)
    {
        ReadMail();
        var results = new List<string>();
        using (var context = new Entities())
        {
            var now = DateTime.Now;
            var limite = now.AddMinutes(-5);
            foreach (var message in context.Messages.Where(x => x.To == user && (x.Read == null || x.Read >limite)))
            {
                results.Add(string.Format("From: {0} at {1}, {2}", message.From, message.Received.ToShortTimeString(), message.Message1));
                message.Read = now;
            }
            context.SaveChanges();
        }

        if (!results.Any())
        {
            results.Add(string.Format("{0} has no new messages..."));
        }

        return results;
    }

    private void ReadMail()
    {
        var client = new Pop3Client();
        client.Connect("mail.epicsportexchange.com", "Message@epicsportexchange.com", "vRa77330");
        var messages = client.List().Where(x => !x.Retrieved).ToList();
        if (!messages.Any())
        {
            client.Disconnect();
            return;
        }
        using (var context = new Entities())
        {
            foreach (var message in messages)
            {
                client.Retrieve(message);
                if (message.Subject != "Mirror")
                {
                    client.Delete(message);
                    continue;
                }
                var entry = new Message();
                entry.Received = DateTime.Parse(message.Date);
                entry.From = MapFrom(message.From);
                var body = message.Body.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (body.Count() < 2)
                {
                    client.Delete(message);
                    continue;
                }
                entry.To = body[0];
                entry.Message1 = body[1];
                context.Messages.Add(entry);
                client.Delete(message);
            }
            context.SaveChanges();
        }
        client.Disconnect();
    }

    private string MapFrom(string from)
    {
        switch (from)
        {
            case "xavier desoindre <xavier.desoindre@gmail.com>":
                return "Xavier";
        }
        return from;
    }
}
