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

namespace HOTAYI_MWMS
{
    class RackInfo
    {
        public string rackID { get; set; }
        public int rackSize { get; set; }
        public int rackCol { get; set; }
        public int rackRow { get; set; }
        public string[] itemsStored { get; set; }
    }
}