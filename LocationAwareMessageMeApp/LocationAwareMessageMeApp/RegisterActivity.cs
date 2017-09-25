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
using Android.Net;
using Android.Support.Design.Widget;
using LocationAwareMessageMeApp.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Android.Util;
using EstimoteSdk;

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "Register")]
    
    public class RegisterActivity : Activity
    {
        EditText etFirstName;
        EditText etLastName;
        EditText etUserName;
        EditText etPassword;
        EditText etConfirmPassword;
        Button btnRegister;
        ProgressDialog _progressDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Register);

            SystemRequirementsChecker.CheckWithDefaultDialogs(this);

            Init();

            SetIcon();
        }

        private void Init()
        {
            etFirstName = FindViewById<EditText>(Resource.Id.firstname);
            etLastName = FindViewById<EditText>(Resource.Id.lastname);
            etUserName = FindViewById<EditText>(Resource.Id.username);
            etPassword = FindViewById<EditText>(Resource.Id.password);
            etConfirmPassword = FindViewById<EditText>(Resource.Id.confirmPassword);
            btnRegister = FindViewById<Button>(Resource.Id.register);
            btnRegister.Click += OnRegisterClicked;
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string firstName = etFirstName.Text;
            string lastName = etLastName.Text;
            string userName = etUserName.Text;
            string password = etPassword.Text;
            string confirmPassword = etConfirmPassword.Text;

            if (!IsInValidInput())
            {
                ConnectivityManager service = (ConnectivityManager)GetSystemService(ConnectivityService);
                NetworkInfo info = service.ActiveNetworkInfo;
                if (info != null)
                {
                    ShowProgress("Working...");
                    var user = new RegisterBindingModel
                    {
                        UserName = userName,
                        FirstName = firstName,
                        LastName = lastName,
                        Password = password,
                        ConfirmPassword = confirmPassword
                    };

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        HttpResponseMessage result = await client.PostAsync(Constants.REGISTER_URL, new FormUrlEncodedContent(user.ToMap()));

                        if (result.IsSuccessStatusCode)
                        {
                            Toast.MakeText(this, "Registration Successful", ToastLength.Short).Show();
                            //Intent GoBackToLogin = new Intent(this, typeof(LoginActivity));
                            //StartActivity(GoBackToLogin);
                            EndProgress();
                            Finish();
                        }
                        else
                        {
                            EndProgress();
                            Toast.MakeText(this, "Registration NOT Successful", ToastLength.Short).Show();
                            Log.Debug(Constants.TAG, "Error occured");
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
            bool isInvalid = false;

            if(etFirstName.Text == null)
            {
                isInvalid = true;
                etFirstName.Error = "First name cannot be empty";
            }
            if (etLastName.Text == null)
            {
                isInvalid = true;
                etLastName.Error = "Last name cannot be empty";
            }
            if (etPassword.Text == null)
            {
                isInvalid = true;
                etPassword.Error = "Password cannot be empty";
            }
            if (etUserName.Text == null)
            {
                isInvalid = true;
                etUserName.Error = "User name cannot be empty";
            }
            if (etConfirmPassword.Text == null)
            {
                isInvalid = true;
                etConfirmPassword.Error = "Confirm Password cannot be empty";
            }
           
            if (!etPassword.Text.Equals(etConfirmPassword.Text))
            {
                isInvalid = true;
                etPassword.Error = "Passwords must match!";
                etConfirmPassword.Error = "Passwords must match!";
            }
           
            return isInvalid;
            
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