using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using Datas.Entities;
using SignalR;

public partial class Fountain : Page
{
    private readonly Dictionary<string, Tuple<int, int, int>> _fountains = new Dictionary<string, Tuple<int, int, int>>();
    private int _fountainsCount = 0;
    private int _worldWideFountainsCount = 0;
    public int N { get; set; }
    private DateTime _referenceDate = DateTime.MinValue;
    protected void Page_Load(object sender, EventArgs e)
    {
        N = -1;
        foreach (string query in Request.QueryString)
        {
            switch (query)
            {
                case "n":
                    int n;
                    if (int.TryParse(Request.QueryString[query], out n))
                    {
                        N = n;
                        _referenceDate = DateTime.Now.AddDays(-N);
                    }
                    break;
            }
        }
    }

    public Dictionary<string, Tuple<int, int, int>> Fountains { get { return _fountains; } }
    public int FountainsCount { get { return _fountainsCount; } }
    public int WorldWideFountainsCount { get { return _worldWideFountainsCount; } }

    protected void Save(object sender, EventArgs e)
    {
        if (Upload.PostedFile != null && !string.IsNullOrEmpty(Upload.PostedFile.FileName) && (Antoine.Checked || Camille.Checked || Loic.Checked || Rafaela.Checked || Xavier.Checked))
        {
            var longitude = FormatDouble(Longitude.Text);
            var lattitude = FormatDouble(Lattitude.Text);

            int value = 0;
            if (Antoine.Checked)
                value += 1;
            if (Camille.Checked)
                value += 2;
            if (Loic.Checked)
                value += 4;
            if (Xavier.Checked)
                value += 8;
            if (Rafaela.Checked)
                value += 16;

            //To create a PostedFile
            HttpPostedFile file = Upload.PostedFile;
            string targetFolder = HttpContext.Current.Server.MapPath("~/Zurich/Images");
            string targetPath = Path.Combine(targetFolder, string.Format("Fountain-{0},{1}-{2}.jpg", String.Format("{0:0.00000000}", longitude), String.Format("{0:0.00000000}", lattitude), value));
            if (File.Exists(targetPath))
            {
                return;
            }

            file.SaveAs(targetPath);
            FountainHub.InsertFountain(Antoine.Checked, Camille.Checked, Loic.Checked, Rafaela.Checked, Xavier.Checked, NotZurich.Checked, longitude, lattitude);
        }
    }

    private double FormatDouble(string text)
    {
        double result;
        if (double.TryParse(text, out result))
        {
            return Math.Round(result, 8);
        }

        if (double.TryParse(text.Replace(",","."), out result))
        {
            return Math.Round(result, 8);
        }

        if (double.TryParse(text.Replace(".",","), out result))
        {
            return Math.Round(result, 8);
        }

        throw new ArgumentException();
    }

    protected void LoadStats()
    {
        _fountains.Add("Antoine", new Tuple<int, int, int>(0,0,0));
        _fountains.Add("Camille", new Tuple<int, int, int>(0, 0, 0));
        _fountains.Add("Loïc", new Tuple<int, int, int>(0, 0, 0));
        _fountains.Add("Rafaela", new Tuple<int, int, int>(0, 0, 0));
        _fountains.Add("Xavier", new Tuple<int, int, int>(0, 0, 0));

        using (var context = new Entities())
        {
            foreach (var fountain in context.Fountains)
            {
                _worldWideFountainsCount++;
                if (fountain.InZurich)
                    _fountainsCount++;
                fountain.Update(_fountains, _referenceDate);
            }
        }
    }
}
