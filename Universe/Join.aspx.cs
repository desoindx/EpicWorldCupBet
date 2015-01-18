using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
        int id;
        if (IsValid && Int32.TryParse(UniverseId.Text, out id))
        {
            using (var context = new Entities())
            {
                var universe = context.Universes.FirstOrDefault(x => x.Id == id && x.Password == Password.Text);
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
                    context.SaveChanges();
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                }
                else
                {
                    FailureText.Text = "Invalid username or password.";
                    ErrorMessage.Visible = true;
                }
            }
        }
    }
}