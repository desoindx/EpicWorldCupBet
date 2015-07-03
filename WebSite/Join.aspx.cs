using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datas.Entities;
using WorldCupBetting;

public partial class Universe_Join : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        foreach (string query in Request.QueryString)
        {
            switch (query)
            {
                case "Id" :
                    UniverseId.Text = Request.QueryString[query];
                    break;
                case "Password" :
                    Password.Text = Request.QueryString[query];
                    break;
            }
        }
    }

    protected void JoinUniverse(object sender, EventArgs e)
    {
        var universeId = UniverseId.Text;
        var password = Password.Text;
        int id;
        if (IsValid && Int32.TryParse(universeId, out id))
        {
            using (var context = new Entities())
            {
                var universe = context.Universes.FirstOrDefault(x => x.Id == id && x.Password == password);
                if (universe != null)
                {
                    var user = Context.User.Identity.Name;
                    if (
                        context.UniverseAvailables.FirstOrDefault(x => x.IdUniverse == universe.Id && x.UserName == user) !=
                        null)
                    {
                        FailureText.Text = "You already are a member of this universe.";
                        ErrorMessage.Visible = true;
                        return;
                    }
                    context.UniverseAvailables.Add(new UniverseAvailable
                    {
                        IdUniverse = universe.Id,
                        UserName = Context.User.Identity.Name
                    });
                    var universeCompetitions = context.UniverseCompetitions.Where(x => x.IdUniverse == universe.Id);
                    foreach (var universeCompetition in universeCompetitions)
                    {
                        context.Moneys.Add(new Money
                        {
                            IdUniverseCompetition = universeCompetition.Id,
                            Money1 = 100000,
                            User = Context.User.Identity.Name
                        });
                    }
                    context.SaveChanges();
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                }
                else
                {
                    FailureText.Text = "Invalid infos.";
                    ErrorMessage.Visible = true;
                }
            }
        }
    }
}