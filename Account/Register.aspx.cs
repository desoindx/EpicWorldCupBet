using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.UI;
using SignalR.SQL;
using WorldCupBetting;

public partial class Account_Register : Page
{
    protected void CreateUser_Click(object sender, EventArgs e)
    {
        if (ModelState.IsValid)
        {
            var manager = new UserManager();
            var user = new ApplicationUser {UserName = UserName.Text};
            IdentityResult result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                IdentityHelper.SignIn(manager, user, isPersistent: false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                Sql.SetUserMoney(user.UserName, 10000);
            }
            else
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
    }
}