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

namespace HOTAYI_MWMS
{
    [Obsolete]
    public class ReceiveFragment : Fragment
    {
        private TextInputLayout inputLayout1, inputLayout2;
        private TextInputEditText input1, input2;
        private Button btn_enter;
        private int c = 0;
        private int counter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.fragment_scanitem, container, false);

            inputLayout1 = view.FindViewById<TextInputLayout>(Resource.Id.tl_input1);
            inputLayout2 = view.FindViewById<TextInputLayout>(Resource.Id.tl_input2);

            //set up hint for received item UI
            inputLayout1.HintEnabled = true;
            inputLayout1.HintAnimationEnabled = true;
            inputLayout1.ExpandedHintEnabled = true;
            inputLayout1.Hint = GetString(Resource.String.partN2);

            inputLayout1.HintEnabled = true;
            inputLayout1.HintAnimationEnabled = true;
            inputLayout1.ExpandedHintEnabled = true;
            inputLayout2.Hint = GetString(Resource.String.qty2);

            input1 = view.FindViewById<TextInputEditText>(Resource.Id.input1);
            input2 = view.FindViewById<TextInputEditText>(Resource.Id.input2);       

            counter = 0;
            
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
                if (!result)
                {
                    inputLayout2.Error = "Please enter or scan a number for quantity";
                    valid = false;
                }

                if (valid)
                {
                    var serialNum = partNum + "_" + qty + "_" + counter;
                    Toast.MakeText(Application.Context, "Part Num: " + partNum + "\nQuantity: " + qty + "\nSerial Num: " + serialNum, ToastLength.Short).Show();
                    saveData(partNum, inp_qty, serialNum);
                    clearInput();
                    c = 0;
                    counter++;
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

        public void saveData(string partN, int qty, string serialN)
        {
            //get shared preferences
            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var json = pref.GetString("Reels", String.Empty);
            List<ReelInfo> reels = JsonConvert.DeserializeObject<List<ReelInfo>>(json);
            //create new object and add into existing list
            ReelInfo reel = new ReelInfo();
            reel.partNum = partN;
            reel.qty = qty;
            reel.serialNum = serialN;
            reels.Add(reel);
            //put the new list into shared preferences
            json = JsonConvert.SerializeObject(reels, Formatting.Indented);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutString("Reels", json);
            editor.Apply();
        }
    }
}