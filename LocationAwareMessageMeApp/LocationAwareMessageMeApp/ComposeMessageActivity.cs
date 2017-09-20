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
using Android.Preferences;
using System.Net.Http;
using Newtonsoft.Json;
using LocationAwareMessageMeApp.Models;
using System.Threading.Tasks;
using Android.Util;

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "ComposeMessageActivity")]
    public class ComposeMessageActivity : Activity
    {
        TextView tvMessageTo;
        TextView tvRegion;
        EditText etMessageBody;
        Button btnSend;
        ImageButton ibListUsers;
        ImageButton ibListRegions;

        ISharedPreferences pref;
        string Access_Token = "";

        private List<Region> Regions;
        private List<User> Users;

        Region SelectedRegion;
        User SelectedUser;
        User CurrentUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ComposeMessage);
            Init();
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
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", Access_Token);

                HttpResponseMessage regions = await client.GetAsync(Constants.GET_REGIONS_URL);
                var users = await client.GetAsync(Constants.GET_USERS_URL);

                if (regions.IsSuccessStatusCode)
                {
                    var jsonRegions = await regions.Content.ReadAsStringAsync();
                    Log.Debug(Constants.TAG, jsonRegions);
                    var regionsList = JsonConvert.DeserializeObject<List<Region>>(jsonRegions);
                    RunOnUiThread(() =>
                    {
                        Regions.Clear();
                        Regions.AddRange(regionsList);
                    });
                }
                else
                {
                    Log.Debug(Constants.TAG, "In else");
                }

                if (users.IsSuccessStatusCode)
                {
                    var jsonUsers = await users.Content.ReadAsStringAsync();
                    Log.Debug(Constants.TAG, jsonUsers);
                    var usersList = JsonConvert.DeserializeObject<List<User>>(jsonUsers);
                    RunOnUiThread(() =>
                    {
                        Users.Clear();
                        Users.AddRange(usersList);
                    });
                }
                else
                {
                    Log.Debug(Constants.TAG, "In else");
                }
                
                
            }
            catch(HttpRequestException exp)
            {
                Log.Error(Constants.TAG, exp.InnerException.Message);
            }
            catch(Exception e)
            {
                Log.Error(Constants.TAG, e.Message);
            }


           

        }

        private void Init()
        {
            pref = PreferenceManager.GetDefaultSharedPreferences(this);
            Access_Token = pref.GetString(Constants.PREF_TOKEN_TAG, "");
            CurrentUser = JsonConvert.DeserializeObject<User>(pref.GetString(Constants.PREF_USER_TAG, ""));

            tvMessageTo = FindViewById<TextView>(Resource.Id.MessageTo);
            tvRegion = FindViewById<TextView>(Resource.Id.Region);
            etMessageBody = FindViewById<EditText>(Resource.Id.ComposeMessage);
            btnSend = FindViewById<Button>(Resource.Id.SendMessage);
            ibListRegions = FindViewById<ImageButton>(Resource.Id.ListRegions);
            ibListUsers = FindViewById<ImageButton>(Resource.Id.ListUsers);

            btnSend.Click += OnSend;
            ibListUsers.Click += OnListUsers;
            ibListRegions.Click += OnListRegions;

            Regions = new List<Region>();
            Users = new List<User>();

        }

        private void OnListRegions(object sender, EventArgs eventArgs)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Regions");
            ArrayAdapter<string> RegionsAdapter = new ArrayAdapter<string>(ApplicationContext, Android.Resource.Layout.SimpleListItem1);
            RegionsAdapter.AddAll(Regions);
            builder.SetAdapter(RegionsAdapter, (s, e) => {
                SelectedRegion = Regions[e.Which];
                tvRegion.Text = "From: " + SelectedRegion.RegionName;
                Log.Debug(Constants.TAG, SelectedRegion.ToString());
            });

            var dialog = builder.Create();
            dialog.Show();
            
        }


        private void OnListUsers(object sender, EventArgs eventArgs)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Users");
            ArrayAdapter<string> UsersAdapter = new ArrayAdapter<string>(ApplicationContext, Android.Resource.Layout.SimpleListItem1);
            UsersAdapter.AddAll(Users);
            builder.SetAdapter(UsersAdapter, (s, e) => {
                SelectedUser = Users[e.Which];
                tvMessageTo.Text = "To: " + SelectedUser.ToString();
                Log.Debug(Constants.TAG, SelectedUser.ToString());
            });

            var dialog = builder.Create();
            dialog.Show();
        }

        private async void OnSend(object sender, EventArgs e)
        {
            try
            {
                if (!IsInValidInput())
                {
                    Models.Message message = new Models.Message
                    {
                        SenderId = CurrentUser.Id,
                        ReceiverId = SelectedUser.Id,
                        MessageBody = etMessageBody.Text,
                        MessageTime = new DateTime().ToString(),
                        IsRead = false
                    };
                    HttpClient client = new HttpClient();

                    StringContent content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8,
                                "application/json");


                    HttpResponseMessage result = await client.PostAsync(Constants.SEND_MESSAGE_URL, new FormUrlEncodedContent(message.ToMap()));

                    if (result.IsSuccessStatusCode)
                    {
                        Toast.MakeText(this, "Message sent!", ToastLength.Short).Show();
                        Intent GoBackToInbox = new Intent(this, typeof(InboxActivity));
                        StartActivity(GoBackToInbox);
                        Finish();
                    }
                    else
                    {
                        Log.Debug(Constants.TAG, "Error occured");
                    }
                }
            }catch(Exception exp)
            {
                Log.Debug(Constants.TAG, exp.Message);
            }
            
        }

        private bool IsInValidInput()
        {
            bool isInvalid = false;
            if(SelectedRegion == null)
            {
                isInvalid = true;  
            }
            if(SelectedUser == null)
            {
                isInvalid = true;
            }
            if(etMessageBody.Text == null)
            {
                isInvalid = true;
            }
            return isInvalid;
            
        }

    }
}