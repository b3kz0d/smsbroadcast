using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SMSBroadcast.Startup))]
namespace SMSBroadcast
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
