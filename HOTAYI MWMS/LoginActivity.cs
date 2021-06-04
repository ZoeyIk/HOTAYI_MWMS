using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.TextField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace HOTAYI_MWMS
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {
        private EditText input_ID, input_password;
        private TextInputLayout inputLayout1, inputLayout2;
        private Button btn_login;
        long lastPress;
        private List<EmpInfo> emp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_login);
            var intent = new Intent(this, typeof(HomeActivity));

            //check if already login
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var empI = sharedPreferences.GetString("EmpInfo", String.Empty);
            //var userID = sharedPreferences.GetString("EmpID", String.Empty);
            //var userPassword = sharedPreferences.GetString("EmpPassword", String.Empty);
            if(empI != "" || empI != String.Empty)
            {
                this.StartActivity(intent);
            }

            input_ID = FindViewById<EditText>(Resource.Id.input_loginID);
            input_password = FindViewById<EditText>(Resource.Id.input_password);
            inputLayout1 = FindViewById<TextInputLayout>(Resource.Id.textInputLayout1);
            inputLayout2 = FindViewById<TextInputLayout>(Resource.Id.textInputLayout2);

            btn_login = FindViewById<Button>(Resource.Id.btn_login);
            btn_login.Click += async delegate
            {
                var empId = input_ID.Text;
                var empPassword = input_password.Text;

                HttpClient client = new HttpClient();
                string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/GetEmpName?empID=" + empId + $"&passW=" + empPassword;
                var uri = new Uri(url);
                HttpResponseMessage responseMessage = await client.GetAsync(uri);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string content = await responseMessage.Content.ReadAsStringAsync();
                    emp = JsonConvert.DeserializeObject<List<EmpInfo>>(content);
                }

                if (emp[0].empName != "FAIL")
                {
                    var user = JsonConvert.SerializeObject(emp, Formatting.Indented);
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
                    ISharedPreferencesEditor editor = pref.Edit();
                    editor.PutString("EmpInfo", user);
                    editor.Apply();
                    this.StartActivity(intent);
                }
                else
                {
                    clearInput();
                    inputLayout1.Error = "Invalid login ID.";
                    inputLayout2.Error = "Invalid password.";
                    Toast.MakeText(this, "Wrong ID or Password", ToastLength.Short);
                }
            };
        }

        public void clearInput()
        {
            input_ID.Text = "";
            input_password.Text = "";
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}