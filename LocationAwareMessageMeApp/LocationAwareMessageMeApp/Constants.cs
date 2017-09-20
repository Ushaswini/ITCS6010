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

namespace LocationAwareMessageMeApp
{
    public static class Constants
    {
        public static readonly string BEARER = "Bearer";
        public static readonly string TAG = "demo";
        public static readonly string PREF_TOKEN_TAG = "access_token";
        public static readonly string PREF_USER_TAG = "user";
        public static readonly string BASE_URL = "http://homework01.azurewebsites.net/";
        public static readonly string LOGIN_URL = BASE_URL + "oauth2/token";
        public static readonly string REGISTER_URL = BASE_URL + "api/Account/Register";
        public static readonly string GET_REGIONS_URL = BASE_URL + "api/Regions";
        public static readonly string GET_USERS_URL = BASE_URL + "api/Users";
        public static readonly string USER_PROFILE_URL = BASE_URL + "api/Account/UserInfo?token={0}";
        public static readonly string SEND_MESSAGE_URL = BASE_URL + "";
    }
}