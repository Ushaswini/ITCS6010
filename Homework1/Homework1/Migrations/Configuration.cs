namespace Homework1.Migrations
{
    using Homework1.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Homework1.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Homework1.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

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
