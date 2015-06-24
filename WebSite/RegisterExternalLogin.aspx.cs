using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Web;
using WorldCupBetting;

public partial class Account_RegisterExternalLogin : System.Web.UI.Page
{
    protected string ProviderName
    {
        get { return (string)ViewState["ProviderName"] ?? String.Empty; }
        private set { ViewState["ProviderName"] = value; }
    }

    protected string ProviderAccountKey
    {
        get { return (string)ViewState["ProviderAccountKey"] ?? String.Empty; }
        private set { ViewState["ProviderAccountKey"] = value; }
    }

    protected void Page_Load()
    {
        // Process the result from an auth provider in the request
        ProviderName = IdentityHelper.GetProviderNameFromRequest(Request);
        if (String.IsNullOrEmpty(ProviderName))
        {
            Response.Redirect("~/Login");
        }
        if (!IsPostBack)
        {
            var loginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo();
            if (loginInfo == null)
            {
                Response.Redirect("~/Login");
            }
            var user = Master.UserManager.Find(loginInfo.Login);
            if (user != null)
            {
                IdentityHelper.SignIn(Master.UserManager, user, isPersistent: false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
            else if (User.Identity.IsAuthenticated)
            {
                // Apply Xsrf check when linking
                var verifiedloginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo(IdentityHelper.XsrfKey, User.Identity.GetUserId());
                if (verifiedloginInfo == null)
                {
                    Response.Redirect("~/Login");
                }

                var result = Master.UserManager.AddLogin(User.Identity.GetUserId(), verifiedloginInfo.Login);
                if (result.Succeeded)
                {
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                }
                else
                {
                    AddErrors(result);
                    return;
                }
            }
            else
            {
                userName.Text = loginInfo.DefaultUserName;
            }
        }
    }

    protected void LogIn_Click(object sender, EventArgs e)
    {
        CreateAndLoginUser();
    }

    private void CreateAndLoginUser()
    {
        if (!IsValid)
        {
            return;
        }
        var user = new ApplicationUser() { UserName = userName.Text };
        IdentityResult result = Master.UserManager.Create(user);
        if (result.Succeeded)
        {
            var loginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo();
            if (loginInfo == null)
            {
                Response.Redirect("~/Login");
                return;
            }
            result = Master.UserManager.AddLogin(user.Id, loginInfo.Login);
            if (result.Succeeded)
            {
                IdentityHelper.SignIn(Master.UserManager, user, isPersistent: false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                return;
            }
            AddErrors(result);
        }
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }
}