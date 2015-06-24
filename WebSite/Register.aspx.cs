using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.UI;
using Microsoft.AspNet.Identity.Owin;
using WorldCupBetting;

public partial class Account_Register : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Master.ShowLogin = false;
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
        /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
           server control at run time. */
    }

    protected async void CreateUser_Click(object sender, EventArgs e)
    {
        if (ModelState.IsValid)
        {
            if (UserName.Text.Contains('@'))
            {
                ErrorMessage.Text = "@ is not allowed in user name.";
            }
            var user = new ApplicationUser { UserName = UserName.Text, Email = Email.Text};
            IdentityResult result = Master.UserManager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                Master.SignInManager.SignIn(user, false, true);
                var code = await Master.UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                var url = new UriBuilder("http://epicworldcupbet.apphb.com/Account/ConfirmEmail");
                var parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters["Id"] = user.Id;
                parameters["Code"] = code;
                url.Query = parameters.ToString();

                await Master.UserManager.SendEmailAsync(user.Id,
                   "Confirm your account",
                   "Please confirm your account by clicking this <a href=\""
                                                   + url + "\">here</a>");
                Response.Redirect("~/Default.aspx");
            }
            else
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
    }
}