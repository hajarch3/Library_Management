using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Gestion_bibliot.Startup))]
namespace Gestion_bibliot
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
