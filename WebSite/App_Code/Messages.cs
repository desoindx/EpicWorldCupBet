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
    public string GetMessages(string user)
    {
        ReadMail();
        return user.ToLower() == "groceries" ? GetGroceries() : GetMessage(user);
    }

    private string GetGroceries()
    {
        var results = string.Empty;
        using (var context = new Entities())
        {
            foreach (var item in context.Groceries)
            {
                results += item + "\n";
            }
        }

        if (results == string.Empty)
        {
            results = "Nothing to buy !";
        }

        return results;
    }

    private static string GetMessage(string user)
    {
        var results = string.Empty;
        using (var context = new Entities())
        {
            user = user.ToLower();
            var now = DateTime.Now;
            var limite = now.AddMinutes(-5);
            foreach (var message in context.Messages.Where(x => (x.To == user || x.To == "all") && (x.Read == null || x.Read > limite)))
            {
                results += string.Format("From: {0} at {1}, {2}\n", message.From, message.Received.ToShortTimeString(),
                    message.Message1);
                if (message.Read == null)
                    message.Read = now;
            }
            context.SaveChanges();
        }

        if (results == string.Empty)
        {
            results = string.Format("{0} has no new messages...", user);
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
                switch (message.Subject)
                {
                    case "Mirror":
                        AddMessage(message, context);
                        break;
                    case "Groceries":
                        AddGrocery(message, context);
                        break;
                }

                client.Delete(message);
            }
            context.SaveChanges();
        }
        client.Disconnect();
    }

    private void AddGrocery(Pop3Message message, Entities context)
    {
        var body =
            message.Body.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                .FirstOrDefault(x => !string.IsNullOrEmpty(x));
        if (body == null)
            return;

        if (body.StartsWith("clear", StringComparison.CurrentCultureIgnoreCase) || body.StartsWith("done", StringComparison.CurrentCultureIgnoreCase))
        {
            var items = context.Groceries.ToList();
            context.Groceries.RemoveRange(items);
            return;
        }

        foreach (var item in body.Split('.'))
        {
            context.Groceries.Add(new Grocery { Item = item });
        }
    }

    private void AddMessage(Pop3Message message, Entities context)
    {
        var entry = new Message();
        entry.Received = DateTime.Parse(message.Date).AddHours(7);
        entry.From = MapFrom(message.From);
        var body =
            message.Body.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        if (body.Count() < 2)
        {
            return;
        }
        entry.To = body[0].ToLower();
        entry.Message1 = body[1];
        context.Messages.Add(entry);
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
