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
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;
using Google.Android.Material.TextField;

namespace HOTAYI_MWMS
{
    public class AddRequestFragment : DialogFragment
    {
        private TextInputLayout inputLayout1, inputLayout2;
        private TextInputEditText input_part, input_qty;
        private Button btn_etr;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.fragment_dialog_request, container, false);

            inputLayout1 = view.FindViewById<TextInputLayout>(Resource.Id.inputLay_req1);
            inputLayout2 = view.FindViewById<TextInputLayout>(Resource.Id.inputLay_req2);
            input_part = view.FindViewById<TextInputEditText>(Resource.Id.input_req_part);
            input_qty = view.FindViewById<TextInputEditText>(Resource.Id.input_req_qty);
            btn_etr = view.FindViewById<Button>(Resource.Id.btn_etr2);

            btn_etr.Click += delegate
            {
                var input_partN = input_part.Text;
                var input_quantity = input_qty.Text;
                bool valid = true;

                if(input_partN == null || input_partN == "")
                {
                    inputLayout1.Error = "Please enter a valid part number";
                    valid = false;
                }
                if(input_quantity == null || input_quantity == "")
                {
                    inputLayout2.Error = "Please enter a quantity";
                    valid = false;
                }

                int.TryParse(input_quantity, out int inp_qty);

                if (valid)
                {
                    var parentAct = this.Activity as RequestActivity;
                    parentAct.addRequest(input_partN, inp_qty);
                    Dismiss();
                }
            };

            return view;
        }
    }
}