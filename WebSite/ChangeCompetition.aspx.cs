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

        Response.Redirect("~/Default.aspx");
        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
    }
}