using System.Configuration;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using WorldCupBetting;

[assembly: OwinStartup(typeof(Startup))]

public partial class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseCookieAuthentication(new CookieAuthenticationOptions
        {
            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            LoginPath = new PathString("/Account/Login")
        });
        app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        app.MapSignalR(); 
    }
}