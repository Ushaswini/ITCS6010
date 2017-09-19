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

namespace LocationAwareMessageMeApp
{
    [Activity(Label = "Inbox")]
    public class InboxActivity : Activity
    {
        ListView lvMessages;

        MessagesAdapter adapter;

        List<Models.Message> Messages;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Inbox);

            Init();

            // TODO get data
        }

        private void Init()
        {
            lvMessages = FindViewById<ListView>(Resource.Id.lvChats);
            Messages = new List<Models.Message>();
            adapter = new MessagesAdapter(this,Messages);
            lvMessages.Adapter = adapter;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            return base.OnMenuItemSelected(featureId, item);
        }


    }
}