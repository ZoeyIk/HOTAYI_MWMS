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

            recyclerView = FindViewById<RecyclerView>(Resource.Id.rc_request);
            manager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(manager);

            btn_addR = FindViewById<ImageButton>(Resource.Id.btn_addR);
            btn_addR.Click += delegate
            {
                FragmentTransaction fragmentT= SupportFragmentManager.BeginTransaction();
                AddRequestFragment addRequest = new AddRequestFragment();
                addRequest.Show(fragmentT, "DialogFragment");
            };

            btn_update = FindViewById<ImageButton>(Resource.Id.btn_updateM);
            btn_update.Click += delegate
            {
                Finish();
            };
        }

        public void addRequest(string partN, int qty)
        {
            var req = new ProdRequest();
            req.partNum = partN;
            req.qty_request = qty;
            if(requests == null)
            {
                requests = new List<ProdRequest>();
            }
            requests.Add(req);

            if(adapter == null)
            {
                adapter = new RequestAdapter(requests);
                recyclerView.SetAdapter(adapter);
            }
            else
            {
                adapter.NotifyDataSetChanged();
            }
        }
    }
}