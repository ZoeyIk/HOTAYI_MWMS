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
using System.Net.Http.Headers;

namespace HOTAYI_MWMS
{
    [Obsolete]
    public class ReceiveFragment : Fragment
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

            //get employee info
            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            emp = JsonConvert.DeserializeObject<List<EmpInfo>>(pref.GetString("EmpInfo", String.Empty));

            inputLayout1 = view.FindViewById<TextInputLayout>(Resource.Id.tl_input1);
            inputLayout2 = view.FindViewById<TextInputLayout>(Resource.Id.tl_input2);

            //set up hint for received item UI
            inputLayout1.HintEnabled = true;
            inputLayout1.HintAnimationEnabled = true;
            inputLayout1.ExpandedHintEnabled = true;
            inputLayout1.Hint = GetString(Resource.String.partN2);

            inputLayout2.HintEnabled = true;
            inputLayout2.HintAnimationEnabled = true;
            inputLayout2.ExpandedHintEnabled = true;
            inputLayout2.Hint = GetString(Resource.String.qty2);

            input1 = view.FindViewById<TextInputEditText>(Resource.Id.input1);
            input2 = view.FindViewById<TextInputEditText>(Resource.Id.input2);

            input1.Click += delegate
            {
                c = 0;
            };
            input2.Click += delegate
            {
                c = 1;
            };

            btn_enter = view.FindViewById<Button>(Resource.Id.btn_etr);
            btn_enter.Click += delegate
            {
                // call API here to insert into database
                var partNum = input1.Text;
                var qty = input2.Text;
                bool valid = true;

                if(partNum == null || partNum == "")
                {
                    inputLayout1.Error = "Please enter or scan a part number";
                    valid = false;
                }
                if(qty == null || qty == "")
                {
                    inputLayout2.Error = "Please enter or scan a quantity";
                    valid = false;
                }

                bool result = int.TryParse(qty, out int inp_qty);
                if (!result || inp_qty <= 0)
                {
                    inputLayout2.Error = "Please enter or scan a number for quantity";
                    valid = false;
                }

                if (valid)
                {
                    saveData(partNum, inp_qty);
                    c = 0;
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

        public async void saveData(string partN, int qty)
        {
            HttpClient client = new HttpClient();
            string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/RecvItems?partN=" + partN + $"&qty=" + qty + $"&empID=" + emp[0].empID;
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
                    inputLayout2.Error = "Quantity has exceeded the limit for one material";
                }
                Toast.MakeText(Application.Context, content, ToastLength.Short).Show();
            }
        }
    }
}