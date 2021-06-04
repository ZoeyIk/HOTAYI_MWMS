using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Android.Material.TextField;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HOTAYI_MWMS
{
    [Activity(Label = "ReportActivity")]
    public class ReportActivity : Activity, View.IOnClickListener
    {
        private TextInputLayout inputLayout;
        private TextInputEditText textInput_partNum;
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager manager;
        private List<ReelInfo> reelInfo;
        private ReportAdapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ISharedPreferences pref2 = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            string data = pref2.GetString("Reels", String.Empty);
            //reelInfo = JsonConvert.DeserializeObject<List<ReelInfo>>(data);

            // Create your application here
            SetContentView(Resource.Layout.activity_report);

            inputLayout = FindViewById<TextInputLayout>(Resource.Id.textInputLayout3);
            textInput_partNum = FindViewById<TextInputEditText>(Resource.Id.input_partNum);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.rc_table);

            inputLayout.SetEndIconOnClickListener(this);

        }

        public async void OnClick(View v)
        {
            var partNum = textInput_partNum.Text;

            if (partNum == null || partNum == "")
            {
                inputLayout.Error = "Please enter a part number";
            }
            else
            {
                //bind data
                HttpClient client = new HttpClient();
                string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/QueryPartNum?partN=" + partNum;
                var uri = new Uri(url);
                HttpResponseMessage responseMessage = await client.GetAsync(uri);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string content = await responseMessage.Content.ReadAsStringAsync();
                    reelInfo = JsonConvert.DeserializeObject<List<ReelInfo>>(content);
                }

                if(reelInfo.Count == 0)
                {
                    inputLayout.Error = "Please enter a valid part number";
                }
                else
                {
                    //set layout manager
                    manager = new LinearLayoutManager(this);
                    recyclerView.SetLayoutManager(manager);

                    //set adapter here
                    adapter = new ReportAdapter(reelInfo, partNum);
                    recyclerView.SetAdapter(adapter);
                }
                
            }

            //throw new NotImplementedException();
        }
    }
}