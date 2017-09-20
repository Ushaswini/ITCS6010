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
                    //do something
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }


    }
}