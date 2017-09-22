using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Preferences;

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "LocationAwareMessageMeApp",MainLauncher =true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ISharedPreferences pref;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.Main);

            pref = PreferenceManager.GetDefaultSharedPreferences(this);
            string login_token = pref.GetString(Constants.PREF_TOKEN_TAG, "");
            if (login_token.Equals(""))
            {
                Intent openLoginScreen = new Intent(this, typeof(LoginActivity));
                StartActivity(openLoginScreen);
                Finish();
            }
            else
            {
                Intent openInboxScreen = new Intent(this, typeof(InboxActivity));
                StartActivity(openInboxScreen);
                Finish();
            }

        }
    }
}

