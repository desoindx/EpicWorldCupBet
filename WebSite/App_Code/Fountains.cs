using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using Datas.Entities;

/// <summary>
/// Summary description for Foutains
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
 [ScriptService]
public class Fountains : WebService
{
    public Fountains()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string Ranking()
    {
        var fountains = new Dictionary<string, Tuple<int, int, int>>
        {
            {"Antoine", new Tuple<int, int, int>(0, 0, 0)},
            {"Camille", new Tuple<int, int, int>(0, 0, 0)},
            {"Loïc", new Tuple<int, int, int>(0, 0, 0)},
            {"Rafaela", new Tuple<int, int, int>(0, 0, 0)},
            {"Xavier", new Tuple<int, int, int>(0, 0, 0)}
        };

        int count = 0;
        using (var context = new Entities())
        {
            foreach (var fountain in context.Fountains)
            {
                count++;
                fountain.Update(fountains, DateTime.MinValue);
            }
        }

        var result = new StringBuilder();
        result.Append("<table><tr><td>Colloc</td><td>First Found</td><td>Found</td></tr>");
        
        foreach (var user in fountains.OrderByDescending(x => x.Value.Item2))
        {
            result.Append("<tr>");
            result.Append(string.Format("<td>{0}</td>", user.Key));
            result.Append(string.Format("<td>{0}</td>", user.Value.Item1));
            result.Append(string.Format("<td>{0}</td>", user.Value.Item2));
            result.Append("</tr>");
        }
        result.Append("<tr>");
        result.Append(string.Format("<td>{0}</td>", "Total"));
        result.Append(string.Format("<td>{0}</td>", "-"));
        result.Append(string.Format("<td>{0}</td>", count));
        result.Append("</tr>");
        result.Append("</table>");
        return result.ToString();
    }

}
