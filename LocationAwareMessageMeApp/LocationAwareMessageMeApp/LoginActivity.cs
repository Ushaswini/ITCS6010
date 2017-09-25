using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net.Http;
using Newtonsoft.Json;
using LocationAwareMessageMeApp.Models;
using Android.Util;
using Android.Preferences;
using Android.Net;
using Android.Support.Design.Widget;
using System.Net.Http.Headers;
using EstimoteSdk;

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        EditText etUserName;
        EditText etPassword;
        Button btnLogin;
        Button btnRegister;
        ProgressDialog _progressDialog;

        ISharedPreferences pref;
        ISharedPreferencesEditor prefEditor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Login);

            SetIcon();

            Init();
        }

        protected override void OnResume()
        {
            base.OnResume();
            SystemRequirementsChecker.CheckWithDefaultDialogs(this);
        }

        private void Init()
        {
            etUserName = FindViewById<EditText>(Resource.Id.username);
            etPassword = FindViewById<EditText>(Resource.Id.password);
            btnLogin = FindViewById<Button>(Resource.Id.login);
            btnRegister = FindViewById<Button>(Resource.Id.signup);
            btnLogin.Click += OnLoginClicked;
            btnRegister.Click += OnRegisterClicked;

            pref = PreferenceManager.GetDefaultSharedPreferences(this);
            prefEditor = pref.Edit();
        }

        private void OnRegisterClicked(object sender, EventArgs e)
        {
            Intent registerUser = new Intent(this, typeof(RegisterActivity));
            StartActivity(registerUser);

        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = etUserName.Text;
            string password = etPassword.Text;

            if (!IsInValidInput())
            {
                //var connMgr = (ConnectivityManager)GetSystemService(ConnectivityService);
                var manager = (ConnectivityManager)GetSystemService(ConnectivityService);
                NetworkInfo info = manager.ActiveNetworkInfo;

                if (info != null)
                {
                    ShowProgress("Trying to login..");

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();

                        Dictionary<string, string> parameters = new Dictionary<string, string>();

                        parameters.Add("grant_type", "password");
                        parameters.Add("username", username);
                        parameters.Add("password", password);

                        try
                        {
                            HttpResponseMessage result = await client.PostAsync(Constants.LOGIN_URL, new FormUrlEncodedContent(parameters));

                            if (result.IsSuccessStatusCode)
                            {
                                string jsonResult = await result.Content.ReadAsStringAsync();
                                // TokenResult is a custom model class for deserialization of the Token Endpoint

                                var resultObject = JsonConvert.DeserializeObject<TokenModel>(jsonResult);

                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resultObject.Access_Token);
                                
                                var profile = await client.GetAsync(String.Format(Constants.USER_PROFILE_URL, resultObject.Access_Token));

                                var jsonProfile = await profile.Content.ReadAsStringAsync();
                                var user = JsonConvert.DeserializeObject<User>(jsonProfile);


                                prefEditor.PutString(Constants.PREF_USER_TAG, JsonConvert.SerializeObject(user));
                                prefEditor.PutString(Constants.PREF_TOKEN_TAG, resultObject.Access_Token);
                                prefEditor.Apply();

                                Intent openInbox = new Intent(this, typeof(InboxActivity));
                                StartActivity(openInbox);
                                EndProgress();
                                Finish();
                            }
                            else
                            {
                                EndProgress();
                                Log.Debug(Constants.TAG, "Error occured");


                                //todo: get message and show
                            }
                        }
                        catch (Exception oExcep)
                        {
                            Log.Error(Constants.TAG, oExcep.Message);
                        }
                    }

                }
                else
                {
                    Snackbar snackBar = Snackbar.Make((Button)sender, "No nternet Connection", Snackbar.LengthIndefinite);
                    //Show the snackbar
                    snackBar.Show();
                }

            }
        }

        private bool IsInValidInput()
        {
            bool isInValid = false;

            if (etUserName.Text == null)
            {
                etUserName.Error = "Username cannot be empty";
                isInValid = true;
            }
            if (etPassword.Text == null)
            {
                etPassword.Error = "Password cannot be empty";
                isInValid = true;
            }

            return isInValid;
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