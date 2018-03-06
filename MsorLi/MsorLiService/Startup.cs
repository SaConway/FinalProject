using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MsorLi.Startup))]

namespace MsorLi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}