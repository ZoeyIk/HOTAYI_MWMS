using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using RecyclerView = AndroidX.RecyclerView.Widget.RecyclerView;

namespace HOTAYI_MWMS
{
    class RequestAdapter : RecyclerView.Adapter
    {
        public event EventHandler<RequestAdapterClickEventArgs> ItemClick;
        public event EventHandler<RequestAdapterClickEventArgs> ItemLongClick;
        List<ProdRequest> list;

        public RequestAdapter(List<ProdRequest> list)
        {
            this.list = list;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.request_item;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new RequestAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            //var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as RequestAdapterViewHolder;
            holder.request_part.Text = list[position].partNum;
            holder.request_qty.Text = list[position].qty_request.ToString();
        }

        public override int ItemCount => list.Count;

        void OnClick(RequestAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(RequestAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class RequestAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView request_part { get; set; }
        public TextView request_qty { get; set; }

        public RequestAdapterViewHolder(View itemView, Action<RequestAdapterClickEventArgs> clickListener,
                            Action<RequestAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            request_part = itemView.FindViewById<TextView>(Resource.Id.request_partN);
            request_qty = itemView.FindViewById<TextView>(Resource.Id.request_qty);

            itemView.Click += (sender, e) => clickListener(new RequestAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RequestAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class RequestAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}