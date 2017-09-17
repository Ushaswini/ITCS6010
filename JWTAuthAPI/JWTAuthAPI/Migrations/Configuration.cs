using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using JWTAuthAPI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JWTAuthAPI.Migrations
{
    public class Configuration : DbMigrationsConfiguration<Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            var user1 = new ApplicationUser()
            {
                UserName = "agibson",
                FirstName = "Austin",
                LastName = "Gibson"
            };
            manager.Create(user1, "test1");
            var user2 = new ApplicationUser()
            {
                UserName = "ssharp",
                FirstName = "Sally",
                LastName = "Sharp"
            };
            manager.Create(user2, "test2");
            var user3 = new ApplicationUser()
            {
                UserName = "preese",
                FirstName = "Phill",
                LastName = "Reese"
            };
            manager.Create(user3, "test3");
            var user4 = new ApplicationUser()
            {
                UserName = "tclark",
                FirstName = "Tracy",
                LastName = "Clark"
            };
            manager.Create(user4, "test4");
            var user5 = new ApplicationUser()
            {
                UserName = "rinc",
                FirstName = "Rebbeca",
                LastName = "Ince"
            };
            manager.Create(user5, "test5");
        }
    }
}