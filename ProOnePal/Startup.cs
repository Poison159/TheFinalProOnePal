using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProOnePal.Startup))]
namespace ProOnePal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
