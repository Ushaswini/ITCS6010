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
        public static readonly string INTENT_TAG = "message";
        public static readonly string REPLY_MESSAGE_TAG = "isToCompose";
        public static readonly string BASE_URL = "http://homework1-group5.azurewebsites.net/";
        public static readonly string LOGIN_URL = BASE_URL + "oauth2/token";
        public static readonly string REGISTER_URL = BASE_URL + "api/Account/Register";
        public static readonly string GET_REGIONS_URL = BASE_URL + "api/Regions";
        public static readonly string GET_USERS_URL = BASE_URL + "api/Users";
        public static readonly string USER_PROFILE_URL = BASE_URL + "api/Account/UserInfo?token={0}";
        public static readonly string SEND_MESSAGE_URL = BASE_URL + "api/Messages";
        public static readonly string GET_MESSAGES_URL = BASE_URL + "api/Messages?receiverId={0}";
        public static readonly string MESSAGE_URL = BASE_URL + "api/Messages/{0}";
        public static readonly string GET_REGION_NAME_URL = BASE_URL + "api/Regions/{0}";
        public static readonly string EDIT_READ_STATUS_URL = BASE_URL + "api/Messages/EditReadStatus?messageId={0}";
        public static readonly string EDIT_LOCK_STATUS_URL = BASE_URL + "api/Messages/EditLockStatus?messageId={0}";
        public static readonly string UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D";
        public static readonly string BEACON_REGIONS_IDENTIFIER = "Beacons_Regions";

        public static readonly string REGION1_IDENTIFIER = "region1";
        public static readonly int MAJOR_REGION1 = 1564;
        public static readonly int MINOR_REGION1 = 34409;

        public static readonly string REGION2_IDENTIFIER = "region2";
        public static readonly int MAJOR_REGION2 = 15212;
        public static readonly int MINOR_REGION2 = 31506;

        public static readonly string REGION3_IDENTIFIER = "region3";
        public static readonly int MAJOR_REGION3 = 26535;
        public static readonly int MINOR_REGION3 = 44799;

    }
}