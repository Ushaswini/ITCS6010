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
using LocationAwareMessageMeApp.Adapters;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Android.Preferences;
using LocationAwareMessageMeApp.Models;
using Android.Util;

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "Inbox")]
    public class InboxActivity : Activity
    {
        ListView lvMessages;
        MessagesAdapter adapter;
        List<Models.Message> Messages;
        ISharedPreferences pref;
        User CurrentUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Inbox);

            Init();

        }

        private void Init()
        {
            Messages = new List<Models.Message>();

            lvMessages = FindViewById<ListView>(Resource.Id.lvChats);
            adapter = new MessagesAdapter(this,Messages);
            lvMessages.Adapter = adapter;
            lvMessages.ItemClick += OnListItemClick;

            pref = PreferenceManager.GetDefaultSharedPreferences(this);        
            CurrentUser = JsonConvert.DeserializeObject<User>(pref.GetString(Constants.PREF_USER_TAG, ""));

        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent readMessage = new Intent(this, typeof(ReadMessageActivity));
            readMessage.PutExtra(Constants.INTENT_TAG, JsonConvert.SerializeObject(Messages[e.Position]));
            StartActivity(readMessage);
        }

        protected async override void OnResume()
        {
            base.OnResume();
            await GetDataAsync();

        }

        protected async Task GetDataAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();

                    string url = String.Format(Constants.GET_MESSAGES_URL, CurrentUser.Id);
                    var data = await client.GetStringAsync(url);
                    var messages = JsonConvert.DeserializeObject<List<Models.Message>>(data);
                    messages = messages.OrderBy(i => i.MessageTime).ToList();

                    RunOnUiThread(() =>
                    {
                        adapter.UpdateData(messages);
                    });
                }
            }catch(Exception e)
            {
                Log.Error(Constants.TAG, e.Message);
            }
           
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.inbox_menu, menu);
            return base.OnPrepareOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.compose:
                    Intent composeMessage = new Intent(this, typeof(ComposeMessageActivity));
                    StartActivity(composeMessage);
                    //do something
                    return true;
                case Resource.Id.refresh:
                    GetDataAsync();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }


    }
}