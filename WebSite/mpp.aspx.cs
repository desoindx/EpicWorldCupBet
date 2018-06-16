using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.UI;
using Microsoft.AspNet.Identity.Owin;
using WorldCupBetting;

public partial class Mpp : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string urlAddress = "https://www.monpetitprono.com/rank/league/KEDBEC69";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

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

            response.Close();
            readStream.Close();
        }
    }
}