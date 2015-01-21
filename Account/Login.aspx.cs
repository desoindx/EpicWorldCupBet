using System.Web;
using Datas.User;
using System;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using WorldCupBetting;

public partial class Account_Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void LogIn(object sender, EventArgs e)
    {
        //            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        //            if (!roleManager.RoleExists("Admin"))
        //            {
        //                var identityResult = roleManager.Create(new IdentityRole("Admin"));
        //                if (identityResult == null)
        //                    return;
        //            }
        if (IsValid)
        {
            // Validate the user password
            ApplicationUser user = Master.UserManager.Find(UserName.Text, Password.Text);
            if (user != null)
            {
                //                    var role = manager.AddToRole(user.Id, "Admin");
                //                    if (role == null)
                //                        return;
                IdentityHelper.SignIn(Master.UserManager, user, RememberMe.Checked);
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