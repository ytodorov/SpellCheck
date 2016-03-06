using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpellCheckMeOnlineWeb.Startup))]
namespace SpellCheckMeOnlineWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
