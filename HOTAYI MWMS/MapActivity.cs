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

            //tableLayout = FindViewById<TableLayout>(Resource.Id.tableLayout1);
            gridLayout = FindViewById<GridLayout>(Resource.Id.gridLayout1);
            int colN = 10; int rowN = 15;

            // Get data from DB, then put into map
            GetMapData(colN, rowN);
            //addTableMapItem(colN, rowN);
            //addGridMapItem(colN, rowN);

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
            gridLayout.AlignmentMode = GridAlign.Margins;
            
            for (int i = 1; i <= rowN; i++)
            {
                //each row (i)
                for (int j = 1; j <= colN; j++)
                {
                    //each column (j)
                    TextView title_rack = new TextView(this);
                    bool path = true;
                    //set the entrance
                    if (i == 1 && j == 1)
                    {
                        title_rack.Gravity = GravityFlags.Center;
                        title_rack.TextAlignment = TextAlignment.Center;
                        path = false;
                        title_rack.SetWidth(80);
                        title_rack.SetBackgroundColor(Color.ParseColor("#FFA200"));
                    }
                    foreach (var rack in racks)
                    {
                        if (rack.rackRow == i && rack.rackCol == j)
                        {
                            title_rack.Text = rack.rackID;
                            title_rack.SetWidth(98);
                            title_rack.Gravity = GravityFlags.Center;
                            if (rack.orientation == "V")
                            {
                                title_rack.Rotation = 90;
                            }
                            title_rack.Background = this.GetDrawable(Resource.Drawable.rectangle);
                            title_rack.TextAlignment = TextAlignment.Center;
                            title_rack.Click += delegate
                            {
                                MapItem_Clicked(rack.rackID, rack.rackSize);
                                title_rack.TextAlignment = TextAlignment.TextEnd;
                            };
                            path = false;
                            break;
                        }
                    }
                    if (path)
                    {
                        title_rack.SetWidth(45);
                        title_rack.SetHeight(45);
                    }
                    title_rack.SetPadding(10, 10, 10, 10);
                    gridLayout.AddView(title_rack);
                }
            }
        }

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

        public async void GetMapData(int col, int row)
        {
            HttpClient client = new HttpClient();
            string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/GetRackInfo";
            var uri = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(uri);
            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                racks = JsonConvert.DeserializeObject<List<RackInfo>>(content);
                addGridMapItem(col, row);
            }
        }
    }
}