using Microsoft.Owin;
using Owin;
using Owin.Security.AesDataProtectorProvider;

[assembly: OwinStartup(typeof(Startup))]

public partial class Startup
{
    public void Configuration(IAppBuilder app)
    {
        ConfigureAuth(app);
        app.UseAesDataProtectorProvider();
        app.MapSignalR();
    }
}