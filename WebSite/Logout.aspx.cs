using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using WorldCupBetting;

public partial class Logout : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        Response.Redirect("~/Default.aspx");
        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
    }
}