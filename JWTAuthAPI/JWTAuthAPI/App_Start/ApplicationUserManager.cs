using JWTAuthAPI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JWTAuthAPI.App_Start
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager() : base(new ApplicationUserStore())
        {
        }

        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store)
        {
        }
    }
}