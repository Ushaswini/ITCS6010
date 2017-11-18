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

namespace iBeaconPracticeApp
{
    public static class Constants
    {
        public static readonly string UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D";
        public static readonly string DISCOUNT_REGION_IDENTIFIER = "Discount_Regions";

        public static readonly string GROCERY_IDENTIFIER = "grocery";
        public static readonly int MAJOR_GROCERY = 15212;
        public static readonly int MINOR_GROCERY = 31506;

        public static readonly string LIFESTYLE_IDENTIFIER = "lifestyle";
        public static readonly int MAJOR_LIFESTYLE = 48071;
        public static readonly int MINOR_LIFESTYLE = 25324;

        public static readonly string PRODUCE_IDENTIFIER = "produce";
        public static readonly int MAJOR_PRODUCE = 26535;
        public static readonly int MINOR_PRODUCE = 44799;

       
        public static readonly string BASE_URL = "http://inclass02-discountsapi.azurewebsites.net/api/Discounts";
        public static readonly string URL = BASE_URL + "?regionName={0}";
    }
}