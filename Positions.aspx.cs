using System;
using System.Web;
using System.Web.UI;
using SignalR.SQL;

public partial class Positions : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
//    protected IHtmlString GetPositions()
//    {
//        return
//            JavaScriptSerializer.SerializeObject(Sql.GetTeamsInformation(Context.User.Identity.Name,
//                Master.SelectedUniverseId,
//                _universeCompetitions[0].Id));
//    }
}