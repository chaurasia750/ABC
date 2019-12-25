using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CBCenter.Startup))]
namespace CBCenter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
