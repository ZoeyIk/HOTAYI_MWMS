using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;

namespace HOTAYI_MWMS
{
    public class MapItemFragment : DialogFragment
    {
        private TextView rackID, rackSize;
        private Button btn_nav;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.map_item, container, false);

            var rackId = Arguments.GetString("rackId");
            var size = Arguments.GetString("rackSize");

            rackID = view.FindViewById<TextView>(Resource.Id.mapItem_rack);
            rackSize = view.FindViewById<TextView>(Resource.Id.tv_mapSize);

            rackID.Text = rackId;
            rackSize.Text = size;

            btn_nav = view.FindViewById<Button>(Resource.Id.btn_nav);
            btn_nav.Click += delegate
            {
                Dismiss();
            };

            return view;
        }
    }
}