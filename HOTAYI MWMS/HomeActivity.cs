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
using AndroidX.CardView;
using AndroidX.CardView.Widget;
using Newtonsoft.Json;

namespace HOTAYI_MWMS
{
    [Activity(Label = "HomeActivity")]
    public class HomeActivity : Activity
    {
        CardView card_scan, card_search, card_map, card_logout;
        TextView empID, empName;
        long lastPress;
        private List<EmpInfo> emp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_home);

            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            emp = JsonConvert.DeserializeObject<List<EmpInfo>>(pref.GetString("EmpInfo", String.Empty));
            empID = FindViewById<TextView>(Resource.Id.empID);
            empName = FindViewById<TextView>(Resource.Id.empName);
            empID.Text = emp[0].empID;
            empName.Text = emp[0].empName;

            card_scan = FindViewById<CardView>(Resource.Id.card_scan);
            card_scan.Click += delegate
            {
                var intent = new Intent(this, typeof(MainActivity));
                this.StartActivity(intent);
            };

            card_search = FindViewById<CardView>(Resource.Id.card_search);
            card_search.Click += delegate
            {
                var intent = new Intent(this, typeof(ReportActivity));
                this.StartActivity(intent);
            };

            card_map = FindViewById<CardView>(Resource.Id.card_map);
            card_map.Click += delegate
            {
                var intent = new Intent(this, typeof(MapActivity));
                this.StartActivity(intent);
            };

            card_logout = FindViewById<CardView>(Resource.Id.card_logout);
            card_logout.Click += delegate
            {
                ISharedPreferencesEditor editor = pref.Edit();
                editor.Clear().Commit();
                var intent = new Intent(this, typeof(LoginActivity));
                this.StartActivity(intent);
            };
        }

        public override void OnBackPressed()
        {
            // source https://stackoverflow.com/a/27124904/3814729
            long currentTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

            // source https://stackoverflow.com/a/14006485/3814729
            if (currentTime - lastPress > 5000)
            {
                Toast.MakeText(this, "Press back again to exit", ToastLength.Long).Show();
                lastPress = currentTime;
            }
            else
            {
                this.FinishAffinity();
            }
        }
    }
}