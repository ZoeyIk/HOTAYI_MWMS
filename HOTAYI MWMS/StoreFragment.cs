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
    public class StoreFragment : Fragment
    {
        private TextInputLayout inputLayout1, inputLayout2;
        private TextInputEditText input1, input2;
        private Button btn_enter;
        private int c = 0;

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

            //set up hint for store item UI
            inputLayout1.HintEnabled = true;
            inputLayout1.HintAnimationEnabled = true;
            inputLayout1.ExpandedHintEnabled = true;
            inputLayout1.Hint = GetString(Resource.String.serialNum2);

            inputLayout1.HintEnabled = true;
            inputLayout1.HintAnimationEnabled = true;
            inputLayout1.ExpandedHintEnabled = true;
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
                    if(saveData(serialN, rack))
                    {
                        Toast.MakeText(Application.Context, "Serial Num: " + serialN + "\nRack ID: " + rack, ToastLength.Short).Show();
                        clearInput();
                        c = 0;
                    }
                    else
                    {
                        inputLayout1.Error = "Please enter or scan a valid serial number";
                    }
                    
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

        public bool saveData(string serialN, string rackId)
        {
            bool exist = false;
            //get shared preferences
            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var json = pref.GetString("Reels", String.Empty);
            List<ReelInfo> reels = JsonConvert.DeserializeObject<List<ReelInfo>>(json);
            //search the existing object in the list, then add in new info
            foreach(var reel in reels)
            {
                if(serialN == reel.serialNum)
                {
                    reel.rackID = rackId;
                    reel.location = "Warehouse";
                    exist = true;
                    break;
                }
            }
            //put the new list into shared preferences
            json = JsonConvert.SerializeObject(reels, Formatting.Indented);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutString("Reels", json);
            editor.Apply();
            return exist;
        }
    }
}