using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Fragment.App;
using AndroidX.Fragment;
using Fragment = AndroidX.Fragment.App.Fragment;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;
using AndroidX.AppCompat.App;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
using System.Net.Http;
using Newtonsoft.Json;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;

namespace HOTAYI_MWMS
{
    [Activity(Label = "RequestActivity")]
    public class RequestActivity : AppCompatActivity
    {
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager manager;
        private RequestAdapter adapter;
        private List<ProdRequest> requests;
        private ImageButton btn_addR, btn_update;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_request);
            SupportActionBar.Title = "Production Request";

            recyclerView = FindViewById<RecyclerView>(Resource.Id.rc_request);
            manager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(manager);

            btn_addR = FindViewById<ImageButton>(Resource.Id.btn_addR);
            btn_addR.Click += delegate
            {
                // add request
                FragmentTransaction fragmentT= SupportFragmentManager.BeginTransaction();
                AddRequestFragment addRequest = new AddRequestFragment();
                addRequest.Show(fragmentT, "DialogFragment");
            };

            btn_update = FindViewById<ImageButton>(Resource.Id.btn_updateM);
            btn_update.Click += delegate
            {
                // pass the request to backend to get rack location
                if(requests!= null)
                {
                    processRequest();
                }
                //Finish();
            };
        }

        public async void addRequest(string partN, int qty)
        {
            if (requests == null)
            {
                requests = new List<ProdRequest>();
            }

            int index = 99;
            bool newItem = true;
            for(int i=0;i < requests.Count;i++)
            {
                if(partN == requests[i].partNum)
                {
                    HttpClient client = new HttpClient();
                    int total = requests[i].qty_request + qty;
                    string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/ValidQTY?partN=" + partN + "&qty=" + total;
                    var uri = new Uri(url);
                    HttpResponseMessage responseMessage = await client.GetAsync(uri);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        string content = await responseMessage.Content.ReadAsStringAsync();
                        if (content == "PASS")
                        {
                            index = i;
                            newItem = false;
                            requests[i].qty_request += qty;
                        }
                        else
                        {
                            Toast.MakeText(Application.Context, "Part number or Quantity is invalid", ToastLength.Short).Show();
                            return;
                        }
                    }
                    break;
                }
            }
            if (newItem)
            {
                var req = new ProdRequest();
                req.partNum = partN;
                req.qty_request = qty;
                HttpClient client = new HttpClient();
                string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/ValidQTY?partN=" + partN + "&qty=" + qty;
                var uri = new Uri(url);
                HttpResponseMessage responseMessage = await client.GetAsync(uri);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string content = await responseMessage.Content.ReadAsStringAsync();
                    if (content == "PASS")
                    {
                        requests.Add(req);
                    }
                    else
                    {
                        Toast.MakeText(Application.Context, "Part number or Quantity is invalid", ToastLength.Short).Show();
                        return;
                    }
                }
                //requests.Sort();
            }

            if (adapter == null)
            {
                adapter = new RequestAdapter(requests);
                recyclerView.SetAdapter(adapter);
            }
            else
            {
                if (!newItem && index != 99)
                    adapter.NotifyItemChanged(index);
                else if (newItem)
                    adapter.NotifyDataSetChanged();
            }
        }

        public async void processRequest()
        {
            HttpClient client = new HttpClient();
            string parts = "";
            string eachQ = "";
            foreach(var r in requests)
            {
                parts += r.partNum + ",";
                eachQ += r.qty_request + ",";
            }
            parts = parts.Substring(0, parts.Length - 1);
            eachQ = eachQ.Substring(0, eachQ.Length - 1);
            string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/ProcessRequests?partNs=" + parts + "&eachQty=" + eachQ;
            var uri = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(uri);
            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                Intent intent = new Intent(this, typeof(MapActivity));
                intent.PutExtra("DestRacks", content);
                SetResult(Result.Ok, intent);
                this.Finish();
            }
        }
    }
}