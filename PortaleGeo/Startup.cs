using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PortaleGeoWeb.Startup))]
namespace PortaleGeoWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
