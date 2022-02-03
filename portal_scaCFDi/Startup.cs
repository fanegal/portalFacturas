using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(portal_scaCFDi.Startup))]
namespace portal_scaCFDi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
