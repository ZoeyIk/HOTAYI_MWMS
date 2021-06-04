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
using System.Net.Http;

namespace HOTAYI_MWMS
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : AppCompatActivity
    {
        private TableLayout tableLayout;
        private GridLayout gridLayout;
        private Button btn_request;
        private List<RackInfo> racks;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_map);

            //Get data from DB

            racks = JsonConvert.DeserializeObject<List<RackInfo>>(json);

            //tableLayout = FindViewById<TableLayout>(Resource.Id.tableLayout1);
            gridLayout = FindViewById<GridLayout>(Resource.Id.gridLayout1);
            int colN = 8; int rowN = 9;

            //addTableMapItem(colN, rowN);
            addGridMapItem(colN, rowN);

            btn_request = FindViewById<Button>(Resource.Id.btn_request);
            btn_request.Click += delegate
            {
                var intent = new Intent(this, typeof(RequestActivity));
                this.StartActivity(intent);
            };
        }

        // generate map using TableLayout
        public void addTableMapItem(int colN, int rowN)
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
                            if (rack.orientation == "V")
                            {
                                title_rack.Rotation = 90;
                                title_rack.SetPadding(10, 10, 10, 10);
                            }
                            else
                            {
                                title_rack.SetPadding(10, 10, 10, 10);
                            }
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

        // generate map using GridLayout
        public void addGridMapItem(int colN, int rowN)
        {
            // set the parameter of the grid layout
            gridLayout.ColumnCount = colN;
            gridLayout.RowCount = rowN;

            for (int i = 1; i <= rowN; i++)
            {
                //each row (i)
                for (int j = 1; j <= colN; j++)
                {
                    //each column (j)
                    TextView title_rack = new TextView(this);
                    //set the entrance
                    if (i == 1 && j == 1)
                    {
                        title_rack.SetWidth(50);
                        title_rack.Gravity = GravityFlags.Center;
                        title_rack.SetBackgroundColor(Color.ParseColor("#FFA200"));
                    }
                    foreach (var rack in racks)
                    {
                        if (rack.rackRow == i && rack.rackCol == j)
                        {
                            title_rack.Text = rack.rackID;
                            title_rack.SetWidth(90);
                            title_rack.Gravity = GravityFlags.Center;
                            if (rack.orientation == "V")
                            {
                                title_rack.Rotation = 90;
                            }
                            title_rack.SetPadding(10, 10, 10, 10);
                            title_rack.Background = this.GetDrawable(Resource.Drawable.rectangle);
                            title_rack.Click += delegate
                            {
                                MapItem_Clicked(rack.rackID, rack.rackSize);
                            };
                            break;
                        }
                    }
                    gridLayout.AddView(title_rack);
                }
            }
        }

        private string json = @"
        [
            { 'rackID': 'Rack1', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 1, 'orientation': 'H' },
            { 'rackID': 'Rack2', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 1, 'orientation': 'H' },
            { 'rackID': 'Rack3', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 1, 'orientation': 'H' },
            { 'rackID': 'Rack4', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 1, 'orientation': 'H' },
            { 'rackID': 'Rack5', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 1, 'orientation': 'H' },
            { 'rackID': 'Rack6', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 1, 'orientation': 'H' },
            { 'rackID': 'Rack7', 'rackSize': 1000, 'rackCol': 8, 'rackRow': 2, 'orientation': 'V' },
            { 'rackID': 'Rack8', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 3, 'orientation': 'H' },
            { 'rackID': 'Rack9', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 3, 'orientation': 'H' },
            { 'rackID': 'Rack10', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 3, 'orientation': 'H' },
            { 'rackID': 'Rack11', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 3, 'orientation': 'H' },
            { 'rackID': 'Rack12', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 3, 'orientation': 'H' },
            { 'rackID': 'Rack40', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 3, 'orientation': 'H' },
            { 'rackID': 'Rack13', 'rackSize': 1000, 'rackCol': 8, 'rackRow': 4, 'orientation': 'V' },
            { 'rackID': 'Rack14', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 4, 'orientation': 'H' },
            { 'rackID': 'Rack15', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 4, 'orientation': 'H' },
            { 'rackID': 'Rack16', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 4, 'orientation': 'H' },
            { 'rackID': 'Rack17', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 4, 'orientation': 'H' },
            { 'rackID': 'Rack18', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 4, 'orientation': 'H' },
            { 'rackID': 'Rack41', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 4, 'orientation': 'H' },
            { 'rackID': 'Rack19', 'rackSize': 1000, 'rackCol': 8, 'rackRow': 6, 'orientation': 'V' },
            { 'rackID': 'Rack20', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 6, 'orientation': 'H' },
            { 'rackID': 'Rack21', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 6, 'orientation': 'H' },
            { 'rackID': 'Rack22', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 6, 'orientation': 'H' },
            { 'rackID': 'Rack23', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 6, 'orientation': 'H' },
            { 'rackID': 'Rack24', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 6, 'orientation': 'H' },
            { 'rackID': 'Rack25', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 6, 'orientation': 'H' },
            { 'rackID': 'Rack42', 'rackSize': 1000, 'rackCol': 8, 'rackRow': 8, 'orientation': 'V' },
            { 'rackID': 'Rack26', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 7, 'orientation': 'H' },
            { 'rackID': 'Rack27', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 7, 'orientation': 'H' },
            { 'rackID': 'Rack28', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 7, 'orientation': 'H' },
            { 'rackID': 'Rack29', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 7, 'orientation': 'H' },
            { 'rackID': 'Rack30', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 7, 'orientation': 'H' },
            { 'rackID': 'Rack43', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 7, 'orientation': 'H' },
            { 'rackID': 'Rack32', 'rackSize': 1000, 'rackCol': 1, 'rackRow': 9, 'orientation': 'H' },
            { 'rackID': 'Rack33', 'rackSize': 1000, 'rackCol': 2, 'rackRow': 9, 'orientation': 'H' },
            { 'rackID': 'Rack34', 'rackSize': 1000, 'rackCol': 3, 'rackRow': 9, 'orientation': 'H' },
            { 'rackID': 'Rack35', 'rackSize': 1000, 'rackCol': 4, 'rackRow': 9, 'orientation': 'H' },
            { 'rackID': 'Rack36', 'rackSize': 1000, 'rackCol': 5, 'rackRow': 9, 'orientation': 'H' },
            { 'rackID': 'Rack37', 'rackSize': 1000, 'rackCol': 6, 'rackRow': 9, 'orientation': 'H' },
            { 'rackID': 'Rack38', 'rackSize': 1000, 'rackCol': 7, 'rackRow': 9, 'orientation': 'H' }
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

        public async void GetMapData()
        {
            HttpClient client = new HttpClient();
            string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/RecvItems?partN=";
            var uri = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(uri);
            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                
            }
        }
    }
}