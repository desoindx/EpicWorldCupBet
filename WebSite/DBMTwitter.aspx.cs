using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBMNotification;
using TweetSharp;

public partial class DBMTwitter : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string password = string.Empty;
        foreach (string query in Request.QueryString)
        {
            switch (query)
            {
                case "p":
                    password = Request.QueryString[query];
                    break;
            }
        }

        string applicationID = StringCipher.Decrypt(
            "xxPuE+4wV9SnLGsum8AH++vOAM2YdGP7LgL7xNNuGbqP8E39p9Z3eokEVlW2jca2wRZxOCDBgoJxfvq8Hau5mzNvmhyWnnDuKpEHoLNtlaaQObMyM2NdwKj56hKRdOA2ibV99PBbuUYLggRU3N8+GjghuDCRoAE/a8uKiQCXcPS6ABjTcTl0XNMCHPAY95rNbG/8f9zIqNY1SOSzLFuYu7b6RKosXyIgkToXn9f3PyodTyPLepEk24zzhnselA1nwQwPdvAVMXRK8gIolDZFC3DSQZEAIQ9AIVtyWU9doIc=",
            password);
        string senderId =
            StringCipher.Decrypt(
                "UXN9wqfTkkaeDCvOva0PZuwsFxAfv9E0unDIZXhqjKqklYtk2pCD1V4mKAqzSK5jYPDv69N6LcWPzPoSK2mb1zF1FvghKppjakQSv/sQJQZZ25UlakPNEN9nfyXqjWqF",
                password);
        var service = new TwitterService(StringCipher.Decrypt(
            "CA89i0TuP6eSmXyghjCyja7yHKcESAgTnSqNiNlS+xYSq46Pw/romSindElRv8JyNPUYa8wR7PfCozZVko4Pdcd19bYKoOcSDl/DwanFcBApB+bcL/Yv2l0Z/vTocfXF",
            password),
            StringCipher.Decrypt(
                "ZVOrKq5cEknsw5Nzfdd/49kcr5XHSbrK1OAt9okMwKLSrGyZixIe+QFKFOllUbETLg1aUX5rNil30iOYXsg3/BqNUR9C/HJpiuGEXDfXkgveMEcKoeUZPL+wdolfgJHrIL9H6SYNtdVb3Q3zj8KEA1gNAkG0BQGMHwrPwv/R/mE=",
                password));
        service.AuthenticateWith(StringCipher.Decrypt(
                "LGn5p3nzuXkUgs3Z+1hxOKsi55MwStAvWG5nLuV0LZckgrKsR/+yhJWGPFoCqucwy5XSd7PP9ADGLkeddEd6+sasenmFoioxMSFyR7h++AXO6iMMlxJT+nDJ95NdZEqCMe9Gf0S4poRusJrTACRg2Hy8zpIUQef1URdl8dO4bNk=",
                password),
            StringCipher.Decrypt(
                "FBDy1L70sxMztW3rRSo86OUWxAPzf2J3Ft78byxr3V67Njq6+EdzWg51cSYrZUdygjRYQbcOinPvZ5z1oUq/8ChqkoqJrmE8Pp2Mnt/l/G0dtvTlBS+AQOLWh7pMm0RHHh2fwfZomxE9k+6LGNaY6/TOGyQmpn9lHSdsrr+NCy0=",
                password));
        var listTweetsOnUserTimelineOptions = new ListTweetsOnUserTimelineOptions
        {
            UserId = 603876321,
            SinceId = LastId()
        };
        var currentTweets = service.ListTweetsOnUserTimeline(listTweetsOnUserTimelineOptions);
        if (currentTweets.Any())
        {
            var tweet = currentTweets.First();
            var text = tweet.Text;
            if (text.Contains("NEW DBM PAGE :"))
            {
                SendNotification(text, applicationID , senderId);
            }

            WriteID(tweet.Id);
        }
    }

    private void SendNotification(string text, string applicationID, string senderId)
    {
        WebRequest request = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        request.Method = "post";
        request.ContentType = "application/json";
        var data = new
        {
            to = "/topics/allDevices",
            notification = new
            {
                body = GetBody(text),
                title = GetTitle(text),
                sound = "enabled"
            }
        };
        var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var json = serializer.Serialize(data);
        Byte[] byteArray = Encoding.UTF8.GetBytes(json);
        request.Headers.Add(string.Format("Authorization: key={0}",
            applicationID));
        request.Headers.Add(string.Format("Sender: id={0}", senderId));
        request.ContentLength = byteArray.Length;

        using (Stream dataStream = request.GetRequestStream())
        {
            dataStream.Write(byteArray, 0, byteArray.Length);
            using (WebResponse tResponse = request.GetResponse())
            {
                using (Stream dataStreamResponse = tResponse.GetResponseStream())
                {
                    using (StreamReader tReader = new StreamReader(dataStreamResponse))
                    {
                        String sResponseFromServer = tReader.ReadToEnd();
                        string str = sResponseFromServer;
                    }
                }
            }
        }
    }

    private string GetTitle(string text)
    {
        var lines = text.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.StartsWith("EN :"))
            {
                return line.Substring(5);
            }
        }

        return lines.First();
    }

    private string GetBody(string text)
    {
        foreach (var line in text.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.Contains("NEW DBM PAGE : "))
            {
                return string.Format("DBM Page {0} is out !", line.Substring(19));
            }
        }

        throw new ArgumentException();
    }

    private long LastId()
    {
        var dbMapPath = HttpContext.Current.Server.MapPath("~/DBM/");
        string file = dbMapPath + "lastID.txt";
        using (var reader = new StreamReader(file))
        {
            return long.Parse(reader.ReadLine());
        }
    }

    private void WriteID(long id)
    {
        var dbMapPath = HttpContext.Current.Server.MapPath("~/DBM/");
        string file = dbMapPath + "lastID.txt";
        File.WriteAllText(file, id.ToString());
    }
}