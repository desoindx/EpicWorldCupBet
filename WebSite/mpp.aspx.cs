using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using Newtonsoft.Json.Linq;

public partial class Mpp : Page
{
    private string _token =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6Im1wZ191c2VyXzEwNDAyOTQiLCJjaGVjayI6IjViZmZlYWIzNDcyNDk0OGUiLCJpYXQiOjE1MjkxNTI5MDJ9.qbqDZ86E5_uDcShZ_yYUnJatodfNaEszU8a0ZAyJZd0";

    public dynamic LastGame;

    public Dictionary<string, string> Teams = new Dictionary<string, string>
    {
        {"RU", "Russie"},
        {"SA", "Arabie Saoudite"},
        {"EG", "Egypte"},
        {"UY", "Uruguay"},
        {"MA", "Maroc"},
        {"IR", "Iran"},
        {"PT", "Portugal"},
        {"ES", "Espagne"},
        {"FR", "France"},
        {"AU", "Australie"},
        {"PE", "Pérou"},
        {"DK", "Danemark"},
        {"AR", "Argentine"},
        {"IS", "Icelande"},
        {"HR", "Croatie"},
        {"NG", "Nigéria"},
        {"RS", "Serbie"},
        {"CR", "Costa Rica"},
        {"BR", "Brésil"},
        {"CH", "Suisse"},
        {"DE", "Allemagne"},
        {"MX", "Mexique"},
        {"SE", "Suède"},
        {"KR", "Corée du sud"},
        {"BE", "Belgique"},
        {"PA", "Panama"},
        {"TN", "Tunisie"},
        {"PL", "Pologne"},
        {"SN", "Sénégal"},
        {"CO", "Colombie"},
        {"JP", "Japon"},
        {"GB-EN", "Angleterre"},
    };

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public List<dynamic> LoadRanking()
    {
        try
        {
            string urlAddress = "https://api.monpetitgazon.com/mpp/KEDBEC69/ranking";

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(urlAddress);

            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", _token);
            request.Accept = "application/json";

            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us,en;q=0.5");
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();
                dynamic d = JObject.Parse(data);
                response.Close();
                readStream.Close();

                return ((JArray) d.topRanking).ToObject<List<dynamic>>().OrderByDescending(x => x.totalScore.Value)
                    .ToList();
            }
        }
        catch { }

        return new List<dynamic>();
    }

    public void LoadForecast()
    {
        try
        {
            string urlAddress = "https://api.monpetitgazon.com/mpp/forecast";

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(urlAddress);

            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", _token);
            request.Accept = "application/json";

            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us,en;q=0.5");
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();
                dynamic d = JObject.Parse(data);
                response.Close();
                readStream.Close();

                var games = new List<dynamic>();
                foreach (JProperty game in ((JObject) d.results).Properties())
                {
                    if (game.Name != "type" && game.Name != "id")
                    {
                        games.Add(game.Value.ToObject<dynamic>());
                    }
                }

                LastGame = games.Where(x => x.startDate.Value < DateTime.UtcNow)
                    .OrderByDescending(x => x.startDate.Value)
                    .First();
            }
        }
        catch { }
    }

    public Dictionary<string, dynamic> LoadProno()
    {
        var pronos = new Dictionary<string, dynamic>();
        pronos["Amaury DD"] = LoadProno("https://api.monpetitgazon.com/mpp/forecast/1039490");
        pronos["Aymeric Souleau"] = LoadProno("https://api.monpetitgazon.com/mpp/forecast/901809");
        pronos["Benoit DB"] = LoadProno("https://api.monpetitgazon.com/mpp/forecast/1039856");
        pronos["John Batman"] = LoadProno("https://api.monpetitgazon.com/mpp/forecast/1040318");
        pronos["Maxime PETIT"] = LoadProno("https://api.monpetitgazon.com/mpp/forecast/1040208");
        pronos["Nicolas Léonard"] = LoadProno("https://api.monpetitgazon.com/mpp/forecast/838356");
        pronos["Samuel Rozé"] = LoadProno("https://api.monpetitgazon.com/mpp/forecast/1039914");
        pronos["Xavier Dsdr"] = LoadProno("https://api.monpetitgazon.com/mpp/forecast/1040294");
        pronos["zakaria REZIKI"] = LoadProno("https://api.monpetitgazon.com/mpp/forecast/1323838");
        return pronos;
    }

    private dynamic LoadProno(string urlAddress)
    {
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(urlAddress);

        request.PreAuthenticate = true;
        request.Headers.Add("Authorization", _token);
        request.Accept = "application/json";

        request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";
        request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us,en;q=0.5");
        HttpWebResponse response = (HttpWebResponse) request.GetResponse();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;

            if (response.CharacterSet == null)
            {
                readStream = new StreamReader(receiveStream);
            }
            else
            {
                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            }

            string data = readStream.ReadToEnd();
            dynamic d = JObject.Parse(data);
            response.Close();
            readStream.Close();

            return ((JArray) d.forecast.values).ToObject<List<dynamic>>().First(x => x.id == LastGame.id);
        }

        return null;
    }
}