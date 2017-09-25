using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Preferences;
using System.Net.Http;
using Newtonsoft.Json;
using LocationAwareMessageMeApp.Models;
using System.Threading.Tasks;
using Android.Util;
using System.Net.Http.Headers;

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "Compose Message")]
    public class ComposeMessageActivity : Activity
    {
        TextView tvMessageTo;
        TextView tvRegion;
        EditText etMessageBody;
        Button btnSend;
        ImageButton ibListUsers;
        ImageButton ibListRegions;
        ProgressDialog _progressDialog;

        ISharedPreferences pref;
        string Access_Token = "";

        private List<Region> Regions;
        private List<User> Users;

        string SelectedRegionId;
        string SelectedUserId;
        User CurrentUser;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ComposeMessage);
            SetIcon();
            ShowProgress("Loading!!");
            await Init();
        }

        protected  override void OnResume()
        {
            base.OnResume();
            
        }

        protected async Task GetDataAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Access_Token);



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
                        EndProgress();
                        Toast.MakeText(this, "Error occured", ToastLength.Short).Show();
                        Finish();
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
                        EndProgress();
                        Toast.MakeText(this, "Error occured", ToastLength.Short).Show();
                        Finish();
                        Log.Debug(Constants.TAG, "In else");
                    }

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

        private async Task Init()
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

            if(Intent.Extras != null)
            {
                if (Intent.Extras.ContainsKey(Constants.REPLY_MESSAGE_TAG))
                {
                    ibListRegions.Enabled = false;
                    ibListUsers.Enabled = false;

                    Models.Message messageToReplyTo = JsonConvert.DeserializeObject<Models.Message>(Intent.Extras.GetString(Constants.REPLY_MESSAGE_TAG));

                    SelectedRegionId = messageToReplyTo.RegionId;
                    SelectedUserId = messageToReplyTo.SenderId;
                    tvMessageTo.Text = "To: " + messageToReplyTo.SenderFullName;
                    tvRegion.Text = "Region: " + messageToReplyTo.RegionName;
                    EndProgress();

                }
            }
            else
            {
                await GetDataAsync().ContinueWith((task) =>
                {
                    RunOnUiThread(() =>
                    {
                        EndProgress();
                    });

                });
            }
            

        }

        private void OnListRegions(object sender, EventArgs eventArgs)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Regions");
            ArrayAdapter<string> RegionsAdapter = new ArrayAdapter<string>(ApplicationContext, Android.Resource.Layout.SimpleListItem1);
            RegionsAdapter.AddAll(Regions);
            builder.SetAdapter(RegionsAdapter, (s, e) => {
                SelectedRegionId = Regions[e.Which].RegionId;
                tvRegion.Text = "From: " + Regions[e.Which].RegionName;
                Log.Debug(Constants.TAG, Regions[e.Which].RegionName);
            });
            

            var dialog = builder.Create();
            dialog.Show();
            //dialog.Window.SetBackgroundDrawableResource(Resource.Color.background_material_dark);

        }

        private void OnListUsers(object sender, EventArgs eventArgs)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Users");
            ArrayAdapter<string> UsersAdapter = new ArrayAdapter<string>(ApplicationContext, Android.Resource.Layout.SimpleListItem1);
            UsersAdapter.AddAll(Users);
            builder.SetAdapter(UsersAdapter, (s, e) => {
                SelectedUserId = Users[e.Which].Id;
                tvMessageTo.Text = "To: " + Users[e.Which].FirstName + " " + Users[e.Which].LastName;
                Log.Debug(Constants.TAG, SelectedUserId.ToString());
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
                    ShowProgress("Sending....");
                    Models.Message message = new Models.Message
                    {
                        SenderId = CurrentUser.Id,
                        ReceiverId = SelectedUserId,
                        MessageBody = etMessageBody.Text,
                        MessageTime = DateTime.Now.ToString("yyyy/MM/dd HH: mm:ss tt"),
                        IsRead = false,
                        IsUnLocked = false,
                        RegionId=SelectedRegionId
                    };

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Access_Token);

                        HttpResponseMessage result = await client.PostAsync(Constants.SEND_MESSAGE_URL, new FormUrlEncodedContent(message.ToMap()));

                        if (result.IsSuccessStatusCode)
                        {
                            EndProgress();
                            Toast.MakeText(this, "Message sent!", ToastLength.Short).Show();
                            Intent GoBackToInbox = new Intent(this, typeof(InboxActivity));
                            GoBackToInbox.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                            StartActivity(GoBackToInbox);
                            Finish();
                        }
                        else
                        {
                            Log.Debug(Constants.TAG, "Error occured");
                            Log.Debug(Constants.TAG, result.ToString());
                            EndProgress();
                            Toast.MakeText(this, "Error occured", ToastLength.Short).Show();
                            Finish();                            
                        }
                    }
                }
            }catch(Exception exp)
            {
                Log.Debug(Constants.TAG, exp.Message);
                Toast.MakeText(this, "Error occured", ToastLength.Short).Show();
                Finish();
            }
            
        }

        private bool IsInValidInput()
        {
            bool isInvalid = false;
            if(SelectedRegionId == null)
            {
                isInvalid = true;  
            }
            if(SelectedUserId == null)
            {
                isInvalid = true;
            }
            if(etMessageBody.Text == null)
            {
                isInvalid = true;
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