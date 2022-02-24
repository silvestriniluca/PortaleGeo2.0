using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NuovoPortaleGeo.Startup))]
namespace NuovoPortaleGeo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
