using JWTAuthAPI.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JWTAuthAPI.App_Start
{
    public class ApplicationUserStore : UserStore<ApplicationUser>
    {
        public ApplicationUserStore (): base(new ApplicationDbContext())
        {

        }
    }
}