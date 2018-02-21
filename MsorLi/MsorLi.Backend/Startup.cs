using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MsorLi.Backend.Startup))]

namespace MsorLi.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}