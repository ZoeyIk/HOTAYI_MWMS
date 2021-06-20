using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using AndroidX.Fragment;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using Google.Android.Material.TextField;
using System.Net.Http;

namespace HOTAYI_MWMS
{
    [Obsolete]
    public class StoreFragment : Fragment
    {
        private TextInputLayout inputLayout1, inputLayout2;
        private TextInputEditText input1, input2;
        private Button btn_enter;
        private int c = 0;
        private List<EmpInfo> emp;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.fragment_scanitem, container, false);

            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            emp = JsonConvert.DeserializeObject<List<EmpInfo>>(pref.GetString("EmpInfo", String.Empty));

            inputLayout1 = view.FindViewById<TextInputLayout>(Resource.Id.tl_input1);
            inputLayout2 = view.FindViewById<TextInputLayout>(Resource.Id.tl_input2);

            //set up hint for store item UI
            inputLayout1.HintEnabled = true;
            inputLayout1.HintAnimationEnabled = true;
            inputLayout1.ExpandedHintEnabled = true;
            inputLayout1.Hint = GetString(Resource.String.serialNum2);

            inputLayout2.HintEnabled = true;
            inputLayout2.HintAnimationEnabled = true;
            inputLayout2.ExpandedHintEnabled = true;
            inputLayout2.Hint = GetString(Resource.String.rackID);

            input1 = view.FindViewById<TextInputEditText>(Resource.Id.input1);
            input2 = view.FindViewById<TextInputEditText>(Resource.Id.input2);

            btn_enter = view.FindViewById<Button>(Resource.Id.btn_etr);
            btn_enter.Click += delegate
            {
                var serialN = input1.Text;
                var rack = input2.Text;
                bool valid = true;

                if (serialN == null || serialN == "")
                {
                    inputLayout1.Error = "Please enter or scan a serial number";
                    valid = false;
                }
                if (rack == null || rack == "")
                {
                    inputLayout2.Error = "Please enter or scan a rack ID";
                    valid = false;
                }
                if (valid)
                {
                    saveData(serialN, rack);
                    c = 0;
                    //inputLayout1.Error = "Please enter or scan a valid serial number"; 
                }
            };

            return view;
        }

        public void setInput(string input)
        {
            if (c % 2 == 0)
            {
                this.input1.Text = input;
            }
            else
            {
                this.input2.Text = input;
            }
            c++;
        }

        public void clearInput()
        {
            input1.Text = "";
            inputLayout1.Error = null;
            input2.Text = "";
            inputLayout2.Error = null;
        }

        public async void saveData(string serialN, string rackId)
        {
            HttpClient client = new HttpClient();
            string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/InsertItems?iQuery=1&serialN=" + serialN + $"&rackOrProd=" + rackId + $"&empID=" + emp[0].empID;
            var uri = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(uri);
            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                if (content == "PASS")
                {
                    clearInput();
                }
                else
                {
                    inputLayout1.Error = "Please enter or scan a valid serial number";
                    inputLayout2.Error = "Please enter or scan a valid rack ID";
                }
                
                Toast.MakeText(Application.Context, content, ToastLength.Short).Show();
            }
            else
            {
                clearInput();
                inputLayout1.Error = "Please enter or scan a valid serial number";
                inputLayout2.Error = "Please enter or scan a valid rack ID";
            }
        }
    }
}