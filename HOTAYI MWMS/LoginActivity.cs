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

namespace HOTAYI_MWMS
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {
        private EditText input_ID, input_password;
        private TextInputLayout inputLayout1, inputLayout2;
        private Button btn_login;
        private string empName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_login);
            var intent = new Intent(this, typeof(HomeActivity));

            //check if already login
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var userID = sharedPreferences.GetString("EmpID", String.Empty);
            var userPassword = sharedPreferences.GetString("EmpPassword", String.Empty);
            if(userID != String.Empty && userPassword != String.Empty)
            {
                var valid = authenticateLogin(userID, userPassword);
                if (valid)
                {
                    this.StartActivity(intent);
                }
            }

            input_ID = FindViewById<EditText>(Resource.Id.input_loginID);
            input_password = FindViewById<EditText>(Resource.Id.input_password);
            inputLayout1 = FindViewById<TextInputLayout>(Resource.Id.textInputLayout1);
            inputLayout2 = FindViewById<TextInputLayout>(Resource.Id.textInputLayout2);

            btn_login = FindViewById<Button>(Resource.Id.btn_login);
            btn_login.Click += delegate
            {
                var valid = authenticateLogin(input_ID.Text, input_password.Text);
                if (valid)
                {
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
                    ISharedPreferencesEditor editor = pref.Edit();
                    editor.PutString("EmpName", empName);
                    editor.PutString("EmpID", input_ID.Text);
                    editor.PutString("EmpPassword", input_password.Text);
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

        public bool authenticateLogin(string userID, string userPassword)
        {
            // if connected to database, here write the authentication 
            if(userID == "10001" && userPassword == "admin")
            {
                empName = "IK ZHU YI";
                return true;
            }
            else
            {
                return false;
            }
        }

        public void clearInput()
        {
            input_ID.Text = "";
            input_password.Text = "";
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}