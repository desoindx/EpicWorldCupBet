using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datas.Entities;
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
    private Competition _currentCompetition;
    private int _id;

    protected void Page_Load(object sender, EventArgs e)
    {
        _currentUniverse = Sql.GetUserSelectedUniverse(Context.User.Identity.Name);
        _currentCompetition = Sql.GetUserSelectedCompetition(_currentUniverse, Context.User.Identity.Name, out _id);

        _numberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        var nfi = _numberFormatInfo;
        nfi.NumberGroupSeparator = " ";
    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut();
    }

    public string SelectedUniverse { get { return _currentUniverse == null ? null : _currentUniverse.Name; } }

    public int SelectedUniverseId { get { return _currentUniverse == null ? -1 : _currentUniverse.Id; } }

    private List<Universe> _userUniverses;

    private List<Universe> UserUniverses
    {
        get { return _userUniverses ?? (_userUniverses = Sql.GetUserUniverses(Context.User.Identity.Name)); }
    }

    private List<Competition> _universeCompetitions;
    private NumberFormatInfo _numberFormatInfo;

    private List<Competition> UniverseCompetitions
    {
        get
        {
            if (_universeCompetitions == null)
            {
                if (SelectedUniverse == null)
                {
                    return new List<Competition>();
                }

                _universeCompetitions = Sql.GetUniverseCompetitions(SelectedUniverse);
            }
            return _universeCompetitions;
        }
    }

    public string GetUniverseCompetition()
    {
        if (_currentCompetition == null)
            return "Welcome " + Context.User.Identity.Name;

        return _currentCompetition.Name;
    }

    public int GetCompetitionId()
    {
        if (_currentCompetition == null)
            return -1;

        return _currentCompetition.Id;
    }

    public int GetUniverseId()
    {
        if (_currentUniverse == null)
            return -1;

        return _currentUniverse.Id;
    }

    public int GetCompetitionUniverseId()
    {
        return _id;
    }

    public bool UserHasUniverse { get { return UserUniverses.Count > 0; } }

    protected void LogIn(object sender, EventArgs e)
    {
        var userName = String.Format("{0}", Request.Form["username"]);
        ApplicationUser user = UserManager.Find(userName, String.Format("{0}", Request.Form["password"]));
        if (user != null)
        {

            SignInManager.SignIn(user, Request.Form["remember"] == "True", true);
            Response.Redirect("~/Default.aspx");
            IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
        }
        else
        {
            FailureText.Text = "Invalid username or password.";
            ErrorMessage.Visible = true;
        }
    }

    protected string GetMoney()
    {
        var money = Sql.GetMoney(Context.User.Identity.GetUserName(), _id);
        return money.ToString("#,##0", _numberFormatInfo);
    }
}
