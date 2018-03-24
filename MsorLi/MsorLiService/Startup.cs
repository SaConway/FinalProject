using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MsorLiService.Startup))]

namespace MsorLiService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}