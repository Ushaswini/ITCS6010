using Microsoft.Owin;
using Owin;
using JWTAuthAPI.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


[assembly: OwinStartup(typeof(JWTAuthAPI.Startup))]

namespace JWTAuthAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
        }
    }
}