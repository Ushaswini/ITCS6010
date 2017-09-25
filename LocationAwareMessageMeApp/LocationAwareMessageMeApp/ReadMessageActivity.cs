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
using System.Threading.Tasks;
using System.Net.Http;
using Android.Preferences;
using LocationAwareMessageMeApp.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Android.Util;
using EstimoteSdk;

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "Read Message")]
    public class ReadMessageActivity : Activity
    {
        TextView tvFrom;
        TextView tvRegion;
        TextView tvMsgBody;
        ProgressDialog _progressDialog;

        Models.Message MessageToRead;
        ISharedPreferences pref;
        string Access_Token;
        User CurrentUser;

        

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ReadMessage);

            SystemRequirementsChecker.CheckWithDefaultDialogs(this);

            ShowProgress("Loading...");

            await Init().ContinueWith((task) =>
            {
                RunOnUiThread(() =>
                {
                    EndProgress();
                });
                
            });

            SetIcon();
        }

         async Task Init()
        {
            var str = Intent.Extras.GetString(Constants.INTENT_TAG);
            MessageToRead = JsonConvert.DeserializeObject<Models.Message>(str);
            pref = PreferenceManager.GetDefaultSharedPreferences(this);
            Access_Token = pref.GetString(Constants.PREF_TOKEN_TAG, "");
            CurrentUser = JsonConvert.DeserializeObject<User>(pref.GetString(Constants.PREF_USER_TAG, ""));

            tvFrom = FindViewById<TextView>(Resource.Id.MessageTo);
            tvRegion = FindViewById<TextView>(Resource.Id.Region);
            tvMsgBody = FindViewById<TextView>(Resource.Id.MessageBody);

            tvMsgBody.Text = MessageToRead.MessageBody;
            tvFrom.Text = "From: " + MessageToRead.SenderFullName;
            tvRegion.Text = "Region: " + MessageToRead.RegionName;
            await GetDataAsync();
        }

        async Task GetDataAsync()
        {
            using (var client = new HttpClient())
            {                
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Access_Token);
               
                HttpContent content = new StringContent("", Encoding.UTF8, "application/json");
                var resp = await client.PostAsync(String.Format(Constants.EDIT_READ_STATUS_URL, MessageToRead.Id), content);
                if (resp.IsSuccessStatusCode)
                {
                    Log.Debug(Constants.TAG, "Edited read status");
                }
            }
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.readmsg_menu, menu);
            return base.OnPrepareOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.reply:
                    Intent replyToMessage = new Intent(this, typeof(ComposeMessageActivity));
                    replyToMessage.PutExtra(Constants.REPLY_MESSAGE_TAG, JsonConvert.SerializeObject(MessageToRead));
                    StartActivity(replyToMessage);
                    //Finish();
                    return true;
                case Resource.Id.delete:
                    ShowDialog();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ShowDialog()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Delete?");
            builder.SetMessage("Do you really want to delete this message");
            builder.SetPositiveButton("Yes", (sender,e) =>{
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Access_Token);
                    var authorizedResponse = client.DeleteAsync(String.Format(Constants.MESSAGE_URL, MessageToRead.Id)).Result;

                    if (authorizedResponse.IsSuccessStatusCode)
                    {
                        //Intent GoToInbox = new Intent(this, typeof(InboxActivity));
                        //StartActivity(GoToInbox);
                        Finish();
                    }
                    else
                    {
                        Toast.MakeText(this, "Error occured. Couldn't be deleted", ToastLength.Short).Show();
                    }
                }
                });
            builder.SetNegativeButton("No", (sender, e) => { });
            AlertDialog dialog = builder.Create();
            dialog.Show();
        }

        private void SetIcon()
        {
            ActionBar.SetDisplayOptions(ActionBarDisplayOptions.ShowTitle,ActionBarDisplayOptions.UseLogo);
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