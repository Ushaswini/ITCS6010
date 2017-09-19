using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;

using Android.Runtime;
using Android.Views;
using Android.Widget;
using Square.Picasso;
using LocationAwareMessageMeApp.Models;

namespace LocationAwareMessageMeApp.Adapters
    {
        public class MessagesAdapter : BaseAdapter<Message>
        {
            protected Activity Context = null;
            protected List<Message> Messages;

            public MessagesAdapter(Activity context, List<Message> messages) : base()
            {
                this.Context = context;
                this.Messages = messages;
            }


            public override long GetItemId(int position)
            {
                return position;
            }

            public override int Count
            {
                get
                {
                    return Messages.Count;
                }
            }

            public override Message this[int index]
            {
                get { return Messages[index]; }
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                ProductViewHolder holder = null;
                var view = convertView;

                if (view == null)
                {
                    view = Context.LayoutInflater.Inflate(Resource.Layout.Message_Row, parent, false);

                    holder = new ProductViewHolder();
                    holder.MessageBody = (TextView)view.FindViewById(Resource.Id.messageBody);
                    holder.MessageTime = (TextView)view.FindViewById(Resource.Id.messageTime);
                    
                    holder.IsLock = (ImageView)view.FindViewById(Resource.Id.isLocked);
                    holder.IsRead = (ImageView)view.FindViewById(Resource.Id.isRead);


                    view.Tag = holder;
                }
                else
                {
                    holder = view.Tag as ProductViewHolder;
                }

                //Now the holder holds reference to our view objects, whether they are 
                //recycled or created new. 
                //Next we need to populate the views

                var item = Messages[position];

                //todo set message length
                holder.MessageBody.Text = item.MessageBody;
                holder.MessageTime.Text = item.MessageTime;

                //tod set image based on is locked and is read

                return view;
            }
            private class ProductViewHolder : Java.Lang.Object
            {
                public TextView MessageBody { get; set; }
                public TextView MessageTime { get; set; }
                public ImageView IsLock { get; set; }
                public ImageView IsRead { get; set; }

            }

            public void UpdateData(List<Message> messages)
            {
                Messages.Clear();
                Messages.AddRange(messages);
                this.NotifyDataSetChanged();
            }
        }
    }
