using Datas.User;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldCupBetting;

public partial class Account_Manage : System.Web.UI.Page
{
    protected string SuccessMessage
    {
        get;
        private set;
    }

    protected bool CanRemoveExternalLogins
    {
        get;
        private set;
    }

    private bool HasPassword(ApplicationUserManager manager)
    {
        var user = manager.FindById(User.Identity.GetUserId());
        return (user != null && user.PasswordHash != null);
    }

    protected void Page_Load()
    {
        if (!IsPostBack)
        {
            // Determine the sections to render
            if (HasPassword(Master.UserManager))
            {
                changePasswordHolder.Visible = true;
            }
            else
            {
                setPassword.Visible = true;
                changePasswordHolder.Visible = false;
            }
            CanRemoveExternalLogins = Master.UserManager.GetLogins(User.Identity.GetUserId()).Count() > 1;

            // Render success message
            var message = Request.QueryString["m"];
            if (message != null)
            {
                // Strip the query string from action
                Form.Action = ResolveUrl("~/Manage");

                SuccessMessage =
                    message == "ChangePwdSuccess" ? "Your password has been changed."
                    : message == "SetPwdSuccess" ? "Your password has been set."
                    : message == "RemoveLoginSuccess" ? "The account was removed."
                    : String.Empty;
                successMessage.Visible = !String.IsNullOrEmpty(SuccessMessage);
            }
        }
    }

    protected void ChangePassword_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            IdentityResult result = Master.UserManager.ChangePassword(User.Identity.GetUserId(), CurrentPassword.Text, NewPassword.Text);
            if (result.Succeeded)
            {
                Response.Redirect("~/Manage?m=ChangePwdSuccess");
            }
            else
            {
                AddErrors(result);
            }
        }
    }

    protected void SetPassword_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            // Create the local login info and link the local account to the user
            IdentityResult result = Master.UserManager.AddPassword(User.Identity.GetUserId(), password.Text);
            if (result.Succeeded)
            {
                Response.Redirect("~/Manage?m=SetPwdSuccess");
            }
            else
            {
                AddErrors(result);
            }
        }
    }

    public IEnumerable<UserLoginInfo> GetLogins()
    {
        var accounts = Master.UserManager.GetLogins(User.Identity.GetUserId());
        CanRemoveExternalLogins = accounts.Count() > 1 || HasPassword(Master.UserManager);
        return accounts;
    }

    public void RemoveLogin(string loginProvider, string providerKey)
    {
        var result = Master.UserManager.RemoveLogin(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        var msg = result.Succeeded
            ? "?m=RemoveLoginSuccess"
            : String.Empty;
        Response.Redirect("~/Manage" + msg);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }
}