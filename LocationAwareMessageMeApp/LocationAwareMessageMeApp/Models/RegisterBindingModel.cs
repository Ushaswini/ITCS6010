using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LocationAwareMessageMeApp.Models
{
    public class RegisterBindingModel
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public Dictionary<string, string> ToMap()
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("UserName", this.UserName);
            parameters.Add("FirstName", this.FirstName);
            parameters.Add("LastName", this.LastName);
            parameters.Add("Password", this.Password);
            parameters.Add("ConfirmPassword", this.ConfirmPassword);
            parameters.Add("Email", this.Email);
            

            return parameters;
        }
    }
}