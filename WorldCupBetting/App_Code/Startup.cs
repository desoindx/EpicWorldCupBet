using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WorldCupBetting.Startup))]
namespace WorldCupBetting
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
