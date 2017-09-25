using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using LocationAwareMessageMeApp.Adapters;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Android.Preferences;
using LocationAwareMessageMeApp.Models;
using Android.Util;
using EstimoteSdk;
using static EstimoteSdk.Utils;
using Android.Support.Design.Widget;

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "Inbox")]
    public class InboxActivity : Activity, BeaconManager.IServiceReadyCallback
    {
        ListView lvMessages;
        MessagesAdapter adapter;
        List<Models.Message> Messages;
        ProgressDialog _progressDialog;

        ISharedPreferences pref;
        User CurrentUser;

        BeaconManager _beaconManager;
        EstimoteSdk.Region _region1;
        EstimoteSdk.Region _region2;
        EstimoteSdk.Region _region3;
        EstimoteSdk.Region _regionBeacons;

        Proximity _proximity;
        double _distance, _currentBeaconDistance;

        long _lastActiveTimeRegion1, _lastActiveTimeRegion2, _lastActiveTimeRegion3, _lastUnlockedTime;
        Beacon _currentBeacon;
        bool isUnlockInProgress;

        string regionId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Inbox);

            SetIcon();

            ShowProgress("Loading!!");

            Init();

        }
        protected async override void OnResume()
        {
            base.OnResume();

            _beaconManager.Connect(this);

            SystemRequirementsChecker.CheckWithDefaultDialogs(this);

            await GetDataAsync().ContinueWith((task) =>
            {
                RunOnUiThread(() => { EndProgress(); });
            });
        }

        public void OnServiceReady()
        {
            // This method is called when BeaconManager is up and running.
            _beaconManager.StartRanging(_region2);
            _beaconManager.StartRanging(_region3);
            _beaconManager.StartRanging(_region1);
            _beaconManager.StartMonitoring(_regionBeacons);

        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            // Make sure we disconnect from the Estimote.
            _beaconManager.Disconnect();
        }

        private void Init()
        {
            Messages = new List<Models.Message>();

            lvMessages = FindViewById<ListView>(Resource.Id.lvChats);
            adapter = new MessagesAdapter(this, Messages);
            lvMessages.Adapter = adapter;
            lvMessages.ItemClick += OnListItemClick;

            pref = PreferenceManager.GetDefaultSharedPreferences(this);
            CurrentUser = JsonConvert.DeserializeObject<User>(pref.GetString(Constants.PREF_USER_TAG, ""));

            InitilizeRegions();

            _beaconManager = new BeaconManager(this);

            _beaconManager.SetForegroundScanPeriod(1500, 2000);

            _beaconManager.Ranging += OnRanging;

            _beaconManager.ExitedRegion += OnExitedRegion;
        }
        protected void InitilizeRegions()
        {
            _regionBeacons = new EstimoteSdk.Region(Constants.BEACON_REGIONS_IDENTIFIER, "B9407F30-F5F8-466E-AFF9-25556B57FE6D");
            _region1 = new EstimoteSdk.Region(Constants.REGION1_IDENTIFIER, "B9407F30-F5F8-466E-AFF9-25556B57FE6D", 1564, 34409);
            _region2 = new EstimoteSdk.Region(Constants.REGION2_IDENTIFIER, "B9407F30-F5F8-466E-AFF9-25556B57FE6D", 15212, 31506);
            _region3 = new EstimoteSdk.Region(Constants.REGION3_IDENTIFIER, "B9407F30-F5F8-466E-AFF9-25556B57FE6D", 26535, 44799);

        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Models.Message selectedMessage = Messages[e.Position];
            if (!selectedMessage.IsUnLocked)
            {
                Toast.MakeText(this, String.Format("Selected message is locked. To unlock, go to region {0}", selectedMessage.RegionName), ToastLength.Short).Show();
            }
            else
            {
                Intent readMessage = new Intent(this, typeof(ReadMessageActivity));
                readMessage.PutExtra(Constants.INTENT_TAG, JsonConvert.SerializeObject(selectedMessage));
                StartActivity(readMessage);
            }

        }
        private async void OnRanging(object sender, BeaconManager.RangingEventArgs e)
        {
            Log.Debug("demo", "IN Ranging " + e.Beacons.Count);
            if (e.Beacons.Count > 0)
            {
                Log.Debug("demo", "List > 0");

                foreach (Beacon beacon in e.Beacons)
                {
                    _proximity = ComputeProximity(beacon);

                    if (_proximity != Proximity.Unknown)
                    {
                        _distance = ComputeAccuracy(beacon);

                        //If distance is valid and (less than current distance or current beacon is null) update current beacon
                        if (_distance > -1 && ((_distance < _currentBeaconDistance) || _currentBeacon == null))
                        {
                            _currentBeaconDistance = _distance;

                            _currentBeacon = beacon;

                            switch (beacon.Major)
                            {
                                case 1564:
                                    _lastActiveTimeRegion1 = DateTime.Now.Ticks;
                                    break;
                                case 15212:
                                    _lastActiveTimeRegion2 = DateTime.Now.Ticks;
                                    break;
                                case 26535:
                                    _lastActiveTimeRegion3 = DateTime.Now.Ticks;
                                    break;
                            }
                        }
                        //If distance is valid and beacon major is same as current beacon's major, update current beacon distance
                        if (_distance > -1 && (beacon.Major == _currentBeacon.Major))
                        {

                            _currentBeaconDistance = _distance;
                            Log.Debug(Constants.TAG, "updating current beacon");

                            if (!isUnlockInProgress)
                            {
                                await UnLockMessagesAsync(_currentBeacon.Major).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }


        }
        private async void OnExitedRegion(object sender, BeaconManager.ExitedRegionEventArgs e)
        {
            //get all messages
            await GetDataAsync();
        }

        protected async Task UnLockMessagesAsync(int majorIdentifier)
        {
            bool unlock = false;
            long currentTime = DateTime.Now.Ticks;
            switch (majorIdentifier)
            {
                case 1564:
                    regionId = "1";
                    unlock = currentTime - _lastActiveTimeRegion1 > 10000 ? true : false;
                    break;
                case 15212:
                    regionId = "2";
                    unlock = currentTime - _lastActiveTimeRegion2 > 10000 ? true : false;
                    break;
                case 26535:
                    regionId = "3";
                    unlock = currentTime - _lastActiveTimeRegion3 > 10000 ? true : false;
                    break;
            }

            if (unlock)
            {

                Log.Debug("TAG", Messages.Count() + "");
                var filteredMessages = from m in Messages where (m.RegionId == regionId && m.IsUnLocked == false) select m;

                Log.Debug("TAG", filteredMessages.Count() + "");

                if (filteredMessages.Count() > 0)
                {
                    isUnlockInProgress = true;
                    RunOnUiThread(() => { ShowProgress("Unlocking messages..."); });
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Access_Token);

                        foreach (Models.Message msg in filteredMessages)
                        {

                            var resp = await client.PostAsync(String.Format(Constants.EDIT_LOCK_STATUS_URL, msg.Id), null).ConfigureAwait(false);
                            if (resp.IsSuccessStatusCode)
                            {
                                Log.Debug(Constants.TAG, "Edited lock status");
                                var editedmsg = Messages.Where(m => m.Id == msg.Id).First();
                                if (editedmsg != null)
                                {
                                    editedmsg.IsUnLocked = true;
                                    RunOnUiThread(() =>
                                    {
                                        adapter.UpdateData(Messages);

                                    });
                                }
                            }
                        }

                        string url = String.Format(Constants.GET_MESSAGES_URL, CurrentUser.Id);
                        var data = await client.GetStringAsync(url).ConfigureAwait(false);
                        var messages = JsonConvert.DeserializeObject<List<Models.Message>>(data);
                        messages = messages.OrderBy(i => i.MessageTime).ToList();
                        Messages.Clear();
                        Messages.AddRange(messages);


                        RunOnUiThread(() =>
                        {
                            adapter.UpdateData(messages);
                            isUnlockInProgress = false;
                            EndProgress();
                        });                       

                    }
                }
            }

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
                    Messages.Clear();
                    Messages.AddRange(messages);
                    messages = messages.OrderBy(i => i.MessageTime).ToList();

                    RunOnUiThread(() =>
                    {
                        adapter.UpdateData(messages);
                    });
                }
            }
            catch (Exception e)
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
                    ShowProgress("Refreshing data");
                    GetDataAsync().ContinueWith((task) => { EndProgress(); });
                    //EndProgress();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
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