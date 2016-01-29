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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Pricer;
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

//
//        var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
//        var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
//        const string roleName = "Admin";
//        var role = new IdentityRole(roleName);
//        var roleresult = roleManager.Create(role);
//        var user = userManager.FindByName("Xavier");
//        var rolesForUser = userManager.GetRoles(user.Id);
//        if (!rolesForUser.Contains(role.Name))
//        {
//            var result = userManager.AddToRole(user.Id, role.Name);
//        }
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

        NumberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        var nfi = NumberFormatInfo;
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
    public NumberFormatInfo NumberFormatInfo;

    protected List<Competition> UniverseCompetitions
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

    protected bool HasMultipleCompetition()
    {
        return UniverseCompetitions.Count > 1;
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
    
    public string GetCompetitionName()
    {
        if (_currentCompetition == null)
            return null;

        return _currentCompetition.Name;
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

            SignInManager.SignIn(user, true, true);
            Response.Redirect("~/Default.aspx");
            IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
        }
        else
        {
            FailureText.Text = "Invalid username or password.";
            ErrorMessage.Visible = true;
        }
    }

    public List<string> GetTeamFor(int competitionId)
    {
        return Sql.GetTeamsForCompetition(competitionId).Select(x => x.Name).ToList();
    }

    public double Money;
    private double? _cashToInvest;

    public string GetMoney()
    {
        Money = Sql.GetMoney(Context.User.Identity.GetUserName(), _id);
        return Money.ToString("#,##0", NumberFormatInfo);
    }

    public string GetCashToInvest()
    {
        if (_cashToInvest == null)
        {
            GetVar();
        }

        return _cashToInvest.Value.ToString("#,##0", NumberFormatInfo);
    }

    public string GetVar()
    {
        var positions = Sql.GetPosition(Context.User.Identity.Name, SelectedUniverseId, GetCompetitionId()).Where(x => x.Value != 0).ToDictionary(x => x.Key, x => x.Value);
        var simulationResults = PricerHelper.GetVars(GetCompetitionName(), positions, new List<double> { 0, 0.1, 0.5, 0.9, 1 });
        var worst10 = simulationResults.Last().Worst10;
        _cashToInvest = Money + worst10;

        return worst10.ToString("#,##0", NumberFormatInfo);
    }
}
