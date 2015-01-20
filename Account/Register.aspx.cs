using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.UI;
using WorldCupBetting;

public partial class Account_Register : Page
{
    protected async void CreateUser_Click(object sender, EventArgs e)
    {
        if (ModelState.IsValid)
        {
            var manager = new UserManager();
            var user = new ApplicationUser {UserName = UserName.Text};
            IdentityResult result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                IdentityHelper.SignIn(manager, user, isPersistent: false);
//                var code = await manager.GenerateEmailConfirmationTokenAsync(user.Id);
//
//                var url = new UrlHelper();
//                var callbackUrl = url.Action("ConfirmEmail", "Account", new {userId = user.Id, code = code},
//                    protocol: Request.Url.Scheme);
//
//                await manager.SendEmailAsync(user.Id,
//                   "Confirm your account",
//                   "Please confirm your account by clicking this link: <a href=\""
//                                                   + callbackUrl + "\">link</a>");
                EmailService.SendSimpleMessage();
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
            else
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
    }
}