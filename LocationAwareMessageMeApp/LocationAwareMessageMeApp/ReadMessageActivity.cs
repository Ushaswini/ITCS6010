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

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "Read Message")]
    public class ReadMessageActivity : Activity
    {
        TextView tvFrom;
        TextView tvRegion;
        TextView tvMsgBody;

        Models.Message MessageToRead;
        ISharedPreferences pref;
        string Access_Token;
        User CurrentUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ReadMessage);
            Init();
            // Create your application here
        }

        private async void Init()
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
            await GetDataAsync();
        }

        private async Task GetDataAsync()
        {
            using (var client = new HttpClient())
            {                
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Access_Token);

                var authorizedResponse = client.GetAsync(String.Format(Constants.GET_REGION_NAME_URL,MessageToRead.RegionId)).Result;

                if (authorizedResponse.IsSuccessStatusCode)
                {
                    var response = await authorizedResponse.Content.ReadAsStringAsync();
                    var region = JsonConvert.DeserializeObject<Models.Region>(response);
                    tvRegion.Text = region.RegionName;
                    //todo: sender name

                    var resp = client.PostAsync(String.Format(Constants.EDIT_READ_STATUS_URL, MessageToRead.Id), null);
                    if (resp.IsCompleted)
                    {
                        Log.Debug(Constants.TAG, "Edited read status");
                    }
                }
                else
                {
                    Toast.MakeText(this, "Error occured", ToastLength.Short).Show();
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
                    Intent composeMessage = new Intent(this, typeof(ComposeMessageActivity));
                    StartActivity(composeMessage);
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
                        Intent GoToInbox = new Intent(this, typeof(InboxActivity));
                        StartActivity(GoToInbox);
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


    }
}