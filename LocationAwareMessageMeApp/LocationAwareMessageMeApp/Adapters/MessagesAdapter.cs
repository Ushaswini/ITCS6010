using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
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

                holder.IsUnLocked = (ImageView)view.FindViewById(Resource.Id.isLocked);
                holder.IsRead = (ImageView)view.FindViewById(Resource.Id.isRead);


                view.Tag = holder;
            }
            else
            {
                holder = view.Tag as ProductViewHolder;
            }
            
            var item = Messages[position];
            
            string text = "";
            if (item.MessageBody.Length > 45)
            {
                text = item.MessageBody.Substring(0, 45);
            }
            else
            {
                text = item.MessageBody;
            }
            holder.MessageBody.Text = text;
            holder.MessageTime.Text = item.MessageTime;

            holder.IsUnLocked.SetImageResource(item.IsUnLocked ? Resource.Drawable.lock_open : Resource.Drawable.locked);
            holder.IsRead.SetImageResource(item.IsRead ? Resource.Drawable.circle_grey : Resource.Drawable.circle_blue);
            
            

            return view;
        }
        private class ProductViewHolder : Java.Lang.Object
        {
            public TextView MessageBody { get; set; }
            public TextView MessageTime { get; set; }
            public ImageView IsUnLocked { get; set; }
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
