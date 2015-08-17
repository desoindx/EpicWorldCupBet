using System;
using SignalR.SQL;
using WorldCupBetting;

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        foreach (string query in Request.QueryString)
        {
            switch (query)
            {
                case "comp":
                    int competitionId;
                    if (int.TryParse(Request.QueryString[query], out competitionId))
                    {
                        Sql.UpdateDefaultCompetition(Context.User.Identity.Name, competitionId);
                    }
                    break;
            }
        }

        var urlReferrer = Request.UrlReferrer;
        if (urlReferrer == null)
        {
            Response.Redirect("~/Default.aspx");
        }
        else
        {
            Response.Redirect(urlReferrer.ToString()); 
        }
        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
    }
}