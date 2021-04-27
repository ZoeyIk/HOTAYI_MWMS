using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using AndroidX.RecyclerView.Widget;
using RecyclerView = AndroidX.RecyclerView.Widget.RecyclerView;
using System.Collections.Generic;

namespace HOTAYI_MWMS
{
    class ReportAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ReportAdapterClickEventArgs> ItemClick;
        public event EventHandler<ReportAdapterClickEventArgs> ItemLongClick;
        private List<ReelInfo> reel; //change to the format of the data
        private string partN;

        public ReportAdapter(List<ReelInfo> reel, string partN)
        {
            this.reel = reel;
            this.partN = partN;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.table_list;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new ReportAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            //here write the code which will be written into view
            //var item = items[position];
            var serialNum = reel[position].serialNum;
            var rack_line = reel[position].rackID;
            if(rack_line == String.Empty || rack_line == null || rack_line == "")
            {
                rack_line = reel[position].prodLine;
            }
            var quantity = reel[position].qty.ToString();
            var loc = reel[position].location;

            // Replace the contents of the view with that element
            var holder = viewHolder as ReportAdapterViewHolder;
            holder.title_partNum.Text = partN;
            holder.query_serialN.Text = serialNum;
            holder.query_rack.Text = rack_line;
            holder.query_qty.Text = quantity;
            holder.query_loc.Text = loc;
        }

        public override int ItemCount => reel.Count;

        void OnClick(ReportAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ReportAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class ReportAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView title_partNum { get; set; }
        public TextView query_serialN { get; set; }
        public TextView query_rack { get; set; }
        public TextView query_qty { get; set; }
        public TextView query_loc { get; set; }

        public ReportAdapterViewHolder(View itemView, Action<ReportAdapterClickEventArgs> clickListener,
                            Action<ReportAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //Declare and bind to the view 
            title_partNum = itemView.FindViewById<TextView>(Resource.Id.title_partNum);
            query_serialN = itemView.FindViewById<TextView>(Resource.Id.query_serialN);
            query_rack = itemView.FindViewById<TextView>(Resource.Id.query_rack);
            query_qty = itemView.FindViewById<TextView>(Resource.Id.query_qty);
            query_loc = itemView.FindViewById<TextView>(Resource.Id.query_loc);

            itemView.Click += (sender, e) => clickListener(new ReportAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ReportAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class ReportAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}