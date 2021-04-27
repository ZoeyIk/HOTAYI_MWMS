using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using AndroidX.RecyclerView.Widget;
using Android.Graphics;
using AndroidX.Fragment.App;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;
using AndroidX.AppCompat.App;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;

namespace HOTAYI_MWMS
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : AppCompatActivity
    {
        private TableLayout tableLayout;
        private Button btn_request;
        private List<RackInfo> racks;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_map);

            racks = JsonConvert.DeserializeObject<List<RackInfo>>(json);

            tableLayout = FindViewById<TableLayout>(Resource.Id.tableLayout1);
            int colN = 8; int rowN = 9;

            addMapItem(colN, rowN);

            btn_request = FindViewById<Button>(Resource.Id.btn_request);
            btn_request.Click += delegate
            {
                var intent = new Intent(this, typeof(RequestActivity));
                this.StartActivity(intent);
            };
        }

        public void addMapItem(int colN, int rowN)
        {
            for (int i = 1; i <= rowN; i++)
            {
                //each row
                TableRow row = new TableRow(this);
                for (int j = 1; j <= colN; j++)
                {
                    //each column
                    TextView title_rack = new TextView(this);
                    foreach (var rack in racks)
                    {
                        if (rack.rackRow == i && rack.rackCol == j)
                        {
                            title_rack.Text = rack.rackID;
                            title_rack.Background = this.GetDrawable(Resource.Drawable.rectangle);
                            title_rack.SetPadding(5, 5, 5, 5);
                            title_rack.Click += delegate
                            {
                                MapItem_Clicked(rack.rackID, rack.rackSize);
                            };
                            break;
                        }
                        else if (i == 1 && j == 1)
                        {
                            title_rack.SetBackgroundColor(Color.ParseColor("#FFA200"));
                        }
                        else
                        {
                            title_rack.Text = "";
                            title_rack.SetPadding(10, 10, 10, 10);
                        }
                    }
                    row.AddView(title_rack);
                }
                tableLayout.AddView(row);
            }
        }

        private string json = @"
        [
            { 'rackID': 'Rack1', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 1 },
            { 'rackID': 'Rack2', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 1 },
            { 'rackID': 'Rack3', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 1 },
            { 'rackID': 'Rack4', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 1 },
            { 'rackID': 'Rack5', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 1 },
            { 'rackID': 'Rack6', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 1 },
            { 'rackID': 'Rack7', 'rackSize': 1000, 'rackCol': 8, 'rackRow': 1 },
            { 'rackID': 'Rack8', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 3 },
            { 'rackID': 'Rack9', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 3 },
            { 'rackID': 'Rack10', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 3 },
            { 'rackID': 'Rack11', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 3 },
            { 'rackID': 'Rack12', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 3 },
            { 'rackID': 'Rack13', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 3 },
            { 'rackID': 'Rack14', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 4 },
            { 'rackID': 'Rack15', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 4 },
            { 'rackID': 'Rack16', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 4 },
            { 'rackID': 'Rack17', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 4 },
            { 'rackID': 'Rack18', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 4 },
            { 'rackID': 'Rack19', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 4 },
            { 'rackID': 'Rack20', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 6 },
            { 'rackID': 'Rack21', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 6 },
            { 'rackID': 'Rack22', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 6 },
            { 'rackID': 'Rack23', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 6 },
            { 'rackID': 'Rack24', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 6 },
            { 'rackID': 'Rack25', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 6 },
            { 'rackID': 'Rack26', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 7 },
            { 'rackID': 'Rack27', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 7 },
            { 'rackID': 'Rack28', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 7 },
            { 'rackID': 'Rack29', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 7 },
            { 'rackID': 'Rack30', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 7 },
            { 'rackID': 'Rack31', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 7 },
            { 'rackID': 'Rack32', 'rackSize': 1000, 'rackCol': 1, 'rackRow': 9 },
            { 'rackID': 'Rack33', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 9 },
            { 'rackID': 'Rack34', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 9 },
            { 'rackID': 'Rack35', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 9 },
            { 'rackID': 'Rack36', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 9 },
            { 'rackID': 'Rack37', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 9 },
            { 'rackID': 'Rack38', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 9 },
            { 'rackID': 'Rack39', 'rackSize': 1000, 'rackCol': 8, 'rackRow': 9 }
        ]";

        public void MapItem_Clicked(string rackID, int rackSize)
        {
            //pass data first
            Bundle bundle = new Bundle();
            bundle.PutString("rackId", rackID);
            bundle.PutString("rackSize", rackSize.ToString());

            FragmentTransaction fragmentT = SupportFragmentManager.BeginTransaction();
            MapItemFragment mapItem = new MapItemFragment();
            mapItem.Arguments = bundle;
            mapItem.Show(fragmentT, "DialogFragment");
        }
    }
}