using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UniteHere.eMemberRegAppWeb.Startup))]
namespace UniteHere.eMemberRegAppWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
