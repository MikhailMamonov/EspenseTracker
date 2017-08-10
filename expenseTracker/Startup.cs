using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(expenseTracker.Startup))]
namespace expenseTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
