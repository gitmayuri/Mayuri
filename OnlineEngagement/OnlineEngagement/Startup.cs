using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineEngagement.Startup))]
namespace OnlineEngagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
