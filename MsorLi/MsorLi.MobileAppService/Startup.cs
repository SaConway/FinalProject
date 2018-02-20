using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MsorLi.MobileAppService.Startup))]

namespace MsorLi.MobileAppService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}