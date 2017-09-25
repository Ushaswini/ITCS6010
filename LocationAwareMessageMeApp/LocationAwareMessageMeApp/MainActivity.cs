using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Preferences;
using EstimoteSdk;

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "MessageMe!",MainLauncher =true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ISharedPreferences pref;
        ProgressDialog _progressDialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.Main);

            SetIcon();

            SystemRequirementsChecker.CheckWithDefaultDialogs(this);

            ShowProgress("Loading...");


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
            EndProgress();

        }

        private void SetIcon()
        {
            ActionBar.SetDisplayOptions(ActionBarDisplayOptions.ShowTitle, ActionBarDisplayOptions.UseLogo);
            ActionBar.SetDisplayShowHomeEnabled(true);
            ActionBar.SetLogo(Resource.Drawable.ic_launcher);
            ActionBar.SetDisplayUseLogoEnabled(true);
        }

        private void ShowProgress(string message)
        {
            _progressDialog = new ProgressDialog(this);
            _progressDialog.SetMessage(message);
            _progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            _progressDialog.SetCancelable(false);
            _progressDialog.Show();
        }

        private void EndProgress()
        {
            _progressDialog.Dismiss();
        }
    }
}

