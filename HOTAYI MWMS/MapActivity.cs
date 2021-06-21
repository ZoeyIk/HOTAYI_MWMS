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
        private GridLayout gridLayout;
        private Button btn_request;
        private List<RackInfo> racks;
        private List<PathInfo> pathInfo;
        private int colN, rowN;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_map);
            SupportActionBar.Title = "Warehouse Map";

            gridLayout = FindViewById<GridLayout>(Resource.Id.gridLayout1);
            colN = 10; rowN = 15;

            // Get data from DB, then put into map
            if (pathInfo == null)
            {
                GetMap(colN, rowN);
            }
            
            btn_request = FindViewById<Button>(Resource.Id.btn_request);
            btn_request.Click += delegate
            {
                var intent = new Intent(this, typeof(RequestActivity));
                this.StartActivityForResult(intent, 0);
            };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            // get racks from request page
            if (resultCode == Result.Ok)
            {
                string resultR = data.GetStringExtra("DestRacks");
                // get path data from db
                //pathInfo = JsonConvert.DeserializeObject<List<PathInfo>>(way);
                //Toast.MakeText(Application.Context, resultR, ToastLength.Short).Show();
                GeneratePath(resultR);
            }
        }

        // create map
        public void createGridMap(int colN, int rowN)
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
                    ImageView img_path = new ImageView(this);
                    bool path = true;
                    if (i == 1 && j == 1)
                    {
                        img_path.SetImageResource(Resource.Drawable.Entrance);
                        path = false;
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
                            if (rack.rackSize >= 600)
                            {
                                title_rack.Background = this.GetDrawable(Resource.Drawable.rect_empty);
                            }
                            else if (rack.rackSize >= 300)
                            {
                                title_rack.Background = this.GetDrawable(Resource.Drawable.rect_half);
                            }
                            else if (rack.rackSize == 0)
                            {
                                title_rack.Background = this.GetDrawable(Resource.Drawable.rect_full);
                            }
                            title_rack.TextAlignment = TextAlignment.Center;
                            title_rack.Click += delegate
                            {
                                MapItem_Clicked(rack.rackID, rack.rackSize);
                            };
                            //path = false;
                            break;
                        }
                    }
                    title_rack.SetPadding(10, 10, 10, 10);
                    if (path)
                        gridLayout.AddView(title_rack);
                    else
                        gridLayout.AddView(img_path);
                }
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
                    ImageView img_path = new ImageView(this);
                    bool path = true;
                    
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
                            if(rack.rackSize >= 500)
                            {
                                title_rack.Background = this.GetDrawable(Resource.Drawable.rect_empty);
                            }
                            else if(rack.rackSize > 0)
                            {
                                title_rack.Background = this.GetDrawable(Resource.Drawable.rect_half);
                            }
                            else if(rack.rackSize == 0)
                            {
                                title_rack.Background = this.GetDrawable(Resource.Drawable.rect_full);
                            }
                            title_rack.TextAlignment = TextAlignment.Center;
                            title_rack.SetPadding(10, 10, 10, 10);
                            title_rack.Click += delegate
                            {
                                MapItem_Clicked(rack.rackID, rack.rackSize);
                            };
                            path = false;
                            break;
                        }
                    }
                    if (path)
                    {
                        bool isPath = true;
                        //set the entrance
                        if (i == 1 && j == 1)
                        {
                            img_path.SetImageResource(Resource.Drawable.Entrance);
                            isPath = false;
                        }
                        if(pathInfo != null || pathInfo.Count > 0)
                        {
                            foreach (var item in pathInfo)
                            {
                                if (i == item.row && j == item.col)
                                {
                                    switch (item.desc)
                                    {
                                        case "NtE":
                                            if (item.destR == "L")
                                                img_path.SetImageResource(Resource.Drawable.NtE_destL);
                                            else if (item.destR == "B")
                                                img_path.SetImageResource(Resource.Drawable.NtE_destB);
                                            else
                                                img_path.SetImageResource(Resource.Drawable.NtE);
                                            break;
                                        case "NtW":
                                            if (item.destR == "R")
                                                img_path.SetImageResource(Resource.Drawable.NtW_destR);
                                            else if (item.destR == "B")
                                                img_path.SetImageResource(Resource.Drawable.NtW_destB);
                                            else
                                                img_path.SetImageResource(Resource.Drawable.NtW);
                                            break;
                                        case "EtS":
                                            if (item.destR == "L")
                                                img_path.SetImageResource(Resource.Drawable.EtS_destTL);
                                            else if (item.destR == "T")
                                                img_path.SetImageResource(Resource.Drawable.EtS_destT);
                                            else
                                                img_path.SetImageResource(Resource.Drawable.EtS);
                                            break;
                                        case "StW":
                                            if (item.destR == "R")
                                                img_path.SetImageResource(Resource.Drawable.StW_destR);
                                            else if (item.destR == "T")
                                                img_path.SetImageResource(Resource.Drawable.StW_destT);
                                            else
                                                img_path.SetImageResource(Resource.Drawable.StW);
                                            break;
                                        case "straightH":
                                            if (item.destR == "T")
                                                img_path.SetImageResource(Resource.Drawable.strH_destT);
                                            else if (item.destR == "B")
                                                img_path.SetImageResource(Resource.Drawable.strH_destB);
                                            else
                                                img_path.SetImageResource(Resource.Drawable.strH);
                                            break;
                                        case "straightV":
                                            if (item.destR == "L")
                                                img_path.SetImageResource(Resource.Drawable.strV_destL);
                                            else if (item.destR == "R")
                                                img_path.SetImageResource(Resource.Drawable.strV_destR);
                                            else
                                                img_path.SetImageResource(Resource.Drawable.strV);
                                            break;
                                    }
                                    isPath = false;
                                }
                            }
                        }
                        if (isPath)
                        {
                            title_rack.SetPadding(10, 10, 10, 10);
                            gridLayout.AddView(title_rack);
                        }
                        else
                            gridLayout.AddView(img_path);
                    }
                    else
                    {
                        img_path = null;
                        gridLayout.AddView(title_rack);
                    }
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

        public async void GetMap(int col, int row)
        {
            HttpClient client = new HttpClient();
            string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/GetRackInfo";
            var uri = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(uri);
            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                racks = JsonConvert.DeserializeObject<List<RackInfo>>(content);
                if(pathInfo == null)
                {
                    createGridMap(col, row);
                }
                else
                {
                    gridLayout.RemoveAllViews();
                    addGridMapItem(col, row);
                }
            }
        }

        public async void GeneratePath(string param)
        {
            // fetch the path data from API
            HttpClient client = new HttpClient();
            string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/RequestPath?destRs=" + param;
            var uri = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(uri);
            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                pathInfo = JsonConvert.DeserializeObject<List<PathInfo>>(content);
                GetMap(colN, rowN);
            }
        }

        public void NavBtnClicked(string rackId)
        {
            GeneratePath(rackId);
        }
    }
}