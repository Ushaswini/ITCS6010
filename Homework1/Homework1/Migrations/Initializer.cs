using Homework1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Homework1.Migrations
{
    internal class Initializer : MigrateDatabaseToLatestVersion<ApplicationDbContext,Configuration>
    {
        //public void InitializeDatabase(ApplicationDbContext context)
        //{
        //    var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

        //    var user1 = new ApplicationUser()
        //    {
        //        UserName = "agibson",
        //        FirstName = "Austin",
        //        LastName = "Gibson"
        //    };
        //    manager.Create(user1, "test1");
        //    var user2 = new ApplicationUser()
        //    {
        //        UserName = "ssharp",
        //        FirstName = "Sally",
        //        LastName = "Sharp"
        //    };
        //    manager.Create(user2, "test2");
        //    var user3 = new ApplicationUser()
        //    {
        //        UserName = "preese",
        //        FirstName = "Phill",
        //        LastName = "Reese"
        //    };
        //    manager.Create(user3, "test3");
        //    var user4 = new ApplicationUser()
        //    {
        //        UserName = "tclark",
        //        FirstName = "Tracy",
        //        LastName = "Clark"
        //    };
        //    manager.Create(user4, "test4");
        //    var user5 = new ApplicationUser()
        //    {
        //        UserName = "rinc",
        //        FirstName = "Rebbeca",
        //        LastName = "Ince"
        //    };
        //    manager.Create(user5, "test5");

        //    var region1 = new Region()
        //    {
        //        RegionId = "1",
        //        RegionName = "Woodward 333F"
        //    };
        //    var region2 = new Region()
        //    {
        //        RegionId = "2",
        //        RegionName = "Woodward 332"
        //    };
        //    var region3 = new Region()
        //    {
        //        RegionId = "3",
        //        RegionName = "Books Stand"
        //    };
            


        //}
    }
}