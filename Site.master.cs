﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datas.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SignalR.SQL;
using WorldCupBetting;

public partial class SiteMaster : MasterPage
{
    public bool ShowLogin = true;
    public ApplicationUserManager UserManager
    {
        get
        {
            return Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }
    }
    public ApplicationSignInManager SignInManager
    {
        get
        {
            return Context.GetOwinContext().Get<ApplicationSignInManager>();
        }
    }

    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    protected void Page_Init(object sender, EventArgs e)
    {
        // The code below helps to protect against XSRF attacks
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // Use the Anti-XSRF token from the cookie
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // Generate a new Anti-XSRF token and save to the cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Set Anti-XSRF token
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // Validate the Anti-XSRF token
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
            }
        }
    }

    private Universe _currentUniverse;
    protected void Page_Load(object sender, EventArgs e)
    {
        _currentUniverse = Sql.GetUserSelectedUniverse(Context.User.Identity.Name);
    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut();
    }

    public string SelectedUniverse { get { return _currentUniverse.Name; } }
    public int SelectedUniverseId { get { return _currentUniverse.Id; } }
    private List<Universe> _userUniverses;
    public List<Universe> UserUniverses
    {
        get
        {
            if (_userUniverses == null)
                _userUniverses = Sql.GetUserUniverses(Context.User.Identity.Name);
            return _userUniverses;
        }
    }

    protected bool UserHasMultipleUniverse()
    {
        var user = Context.User.Identity.Name;
        if (string.IsNullOrEmpty(user))
            return false;

        return UserUniverses.Count > 1;
    }

    protected string GetUserUniverse()
    {
        if (UserUniverses.Count == 1)
            return UserUniverses[0].Name;

        return string.Empty;
    }

    private List<Competition> _universeCompetitions;

    public List<Competition> UniverseCompetitions
    {
        get
        {
            if (_universeCompetitions == null)
                _universeCompetitions = Sql.GetUniverseCompetitions(SelectedUniverse);
            return _universeCompetitions;
        }
    }

    public bool UniverseHasMultipleCompetition()
    {
        return UniverseCompetitions.Count > 1;
    }

    public string GetUniverseCompetition()
    {
        if (UniverseCompetitions != null && UniverseCompetitions.Count == 1)
            return UniverseCompetitions[0].Name;

        return string.Empty;
    }

    public int GetCompetitionId()
    {
        if (UniverseCompetitions != null && UniverseCompetitions.Count == 1)
            return UniverseCompetitions[0].Id;

        return -1;
    }

    public bool UserHasUniverse { get { return UserUniverses.Count > 0; } }

    protected void LogIn(object sender, EventArgs e)
    {
        // so dirty
        var control = Controls[3].Controls[7].Controls[0];
        ApplicationUser user = UserManager.Find(((TextBox)control.FindControl("UserName")).Text,
            ((TextBox)control.FindControl("Password")).Text);
        if (user != null)
        {

            SignInManager.SignIn(user, ((CheckBox)control.FindControl("RememberMe")).Checked, true);
            Response.Redirect("~/Default.aspx");
            IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
        }
        else
        {
            ((Literal)control.FindControl("FailureText")).Text = "Invalid username or password.";
            ((PlaceHolder)control.FindControl("ErrorMessage")).Visible = true;
        }
    }

    protected void SelectNewUniverse(object sender, EventArgs e)
    {
        foreach (var universe in UserUniverses)
            if (universe.Id.ToString(CultureInfo.InvariantCulture) == UniverseId.Text)
            {
                Sql.SetUserUniverse(Context.User.Identity.Name, universe);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                return;
            }
    }
}
