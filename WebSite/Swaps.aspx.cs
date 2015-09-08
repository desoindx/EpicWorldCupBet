using System;
using System.Web;
using System.Web.UI;
using SignalR.SQL;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected IHtmlString GetSwaps(int? id = null)
    {
        if (!id.HasValue)
            id = Master.GetCompetitionId();
        return
            JavaScriptSerializer.SerializeObject(Sql.GetSwaps(Context.User.Identity.Name,
                Master.SelectedUniverseId, id.Value));
    }
}