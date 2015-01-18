﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using SignalR.SQL;
using WorldCupBetting;

public partial class SiteMaster : MasterPage
{
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

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut();
    }

    public string SelectedUniverse { get { return UserHasUniverse ? UserUniverses[0].Name : string.Empty; } }
    public int SelectedUniverseId { get { return UserUniverses[0].Id; } }
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
        var control = Controls[3].Controls[6].Controls[0];
            var manager = new UserManager();
            ApplicationUser user = manager.Find(((TextBox)control.FindControl("UserName")).Text,
            ((TextBox)control.FindControl("Password")).Text);
        if (user != null)
            IdentityHelper.SignIn(manager, user, false);//RememberMe.Checked);
            IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
        }
//        else
//        {
//            FailureText.Text = "Invalid username or password.";
//            ErrorMessage.Visible = true;
//        }
}
