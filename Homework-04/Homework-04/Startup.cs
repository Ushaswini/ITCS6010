﻿using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Hangfire;
using System;

[assembly: OwinStartup(typeof(Homework_04.Startup))]

namespace Homework_04
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            ConfigureAuth(app);

            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("AzureDatabaseConnection");
            BackgroundJob.Enqueue(() => Console.WriteLine("Getting Started with HangFire!"));


            app.UseHangfireDashboard();
            app.UseHangfireServer();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = false;

        }
    }
}
