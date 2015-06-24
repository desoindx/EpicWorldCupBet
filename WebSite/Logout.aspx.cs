using System;
using System.Web;
using Datas.User;
using Microsoft.AspNet.Identity.Owin;
using WorldCupBetting;

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Context.GetOwinContext().Get<ApplicationSignInManager>().AuthenticationManager.SignOut();
        Response.Redirect("~/Default.aspx");
        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
    }
}