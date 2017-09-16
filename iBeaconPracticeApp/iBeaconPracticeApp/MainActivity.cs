using Android.App;
using Android.Widget;
using Android.OS;
using EstimoteSdk;
using System.Net.Http;
using Newtonsoft.Json;
using iBeaconPracticeApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EstimoteSdk.Utils;
using System;
using System.Linq;

namespace iBeaconPracticeApp
{
    [Activity(Label = "Welcome!!", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, BeaconManager.IServiceReadyCallback
    {
        BeaconManager _beaconManager;
        Region _regionProduce;
        Region _regionGrocery;
        Region _regionLifeStyle;
        Region _regionBeacon;

        Proximity _proximity;
        double _distance,_currentBeaconDistance;
        long _lastActiveTime;
        Beacon _currentBeacon;

        ListView lv;
        List<Product> Products;
        ProductsAdapter adapter;

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            Init();

            await GetDataAsync("");

        }        

        protected override void OnResume()
        {
            base.OnResume();
            SystemRequirementsChecker.CheckWithDefaultDialogs(this);
            _beaconManager.Connect(this);
        }

        public void OnServiceReady()
        {
            // This method is called when BeaconManager is up and running.
            _beaconManager.StartRanging(_regionGrocery);
            _beaconManager.StartRanging(_regionLifeStyle);
            _beaconManager.StartRanging(_regionProduce);
            _beaconManager.StartMonitoring(_regionBeacon);

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // Make sure we disconnect from the Estimote.
            _beaconManager.Disconnect();
        }

        protected void InitilizeRegions()
        {
            _regionBeacon = new Region(Constants.DISCOUNT_REGION_IDENTIFIER, Constants.UUID);
            _regionGrocery = new Region(Constants.GROCERY_IDENTIFIER, Constants.UUID, Constants.MAJOR_GROCERY, Constants.MINOR_GROCERY);
            _regionLifeStyle = new Region(Constants.LIFESTYLE_IDENTIFIER, Constants.UUID, Constants.MAJOR_LIFESTYLE, Constants.MINOR_LIFESTYLE);
            _regionProduce = new Region(Constants.PRODUCE_IDENTIFIER, Constants.UUID, Constants.MAJOR_PRODUCE, Constants.MINOR_PRODUCE);
       
        }

        protected  void Init()
        {
            // GetDataAsync("");

            lv = FindViewById<ListView>(Resource.Id.listView);

            Products = new List<Product>();

            adapter = new ProductsAdapter(this, Products);

            lv.Adapter = adapter;

            InitilizeRegions();

            _beaconManager = new BeaconManager(this);

            _beaconManager.SetBackgroundScanPeriod(150,2000);

            _beaconManager.Ranging += OnRanging;

            _beaconManager.ExitedRegion += OnExitedRegion;
        }

        private async void OnRanging(object sender, BeaconManager.RangingEventArgs e)
        {
            if (e.Beacons.Count > 0)
            {
                foreach (Beacon beacon in e.Beacons)
                {

                    _proximity = ComputeProximity(beacon);
                    if (_proximity != Proximity.Unknown)
                    {
                        _distance = ComputeAccuracy(beacon);
                        _lastActiveTime = new DateTime().Ticks;

                        //If distance is valid and (less than current distance or current beacon is null) update current beacon
                        if (_distance > -1 && ((_distance < _currentBeaconDistance) || _currentBeacon == null))
                        {

                            _currentBeaconDistance = _distance;
                            _currentBeacon = beacon;
                        }
                        //If distance is valid and beacon major is same as current beacon's major, update current beacon distance
                        if (_distance > -1 && (beacon.Major == _currentBeacon.Major))
                        {
                            _currentBeaconDistance = _distance;
                        }

                        if (_currentBeacon != null)
                        {
                            await GetDataAsync(_currentBeacon.Major.ToString());
                        }
                    }
                }
            }
            else
            {
                if (((new DateTime()).Ticks - _lastActiveTime) > 5300)
                {
                    await GetDataAsync("");
                }
                _lastActiveTime = (new DateTime()).Ticks;
            }

        }
        private async void OnExitedRegion(object sender, BeaconManager.ExitedRegionEventArgs e)
        {
            await GetDataAsync("");
        }

        protected async Task GetDataAsync(string majorIdentifier)
        {
            string url = "";
            string message = "";
            if (majorIdentifier.Equals(Constants.MAJOR_GROCERY))
            {
                url = string.Format(Constants.URL, Constants.GROCERY_IDENTIFIER);
                message = Constants.GROCERY_IDENTIFIER;
            }else if (majorIdentifier.Equals(Constants.MAJOR_LIFESTYLE))
            {
                url = string.Format(Constants.URL, Constants.LIFESTYLE_IDENTIFIER);
                message = Constants.LIFESTYLE_IDENTIFIER;
            }
            else if (majorIdentifier.Equals(Constants.MAJOR_PRODUCE))
            {
                url = string.Format(Constants.URL, Constants.PRODUCE_IDENTIFIER);
                message = Constants.PRODUCE_IDENTIFIER;
            }
            else
            {
                url = Constants.BASE_URL;
                message = "All items";
            }

            Toast.MakeText(this, message, ToastLength.Long).Show();

            HttpClient client = new HttpClient();
            var data = await client.GetStringAsync(url);
            var products = JsonConvert.DeserializeObject<List<Product>>(data);
            products = products.OrderBy(i => i.RegionId).ToList();

            RunOnUiThread(() => {
                adapter.UpdateData(products);
            });
            
        }
    }
}

