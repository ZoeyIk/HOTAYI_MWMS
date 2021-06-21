using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Fragment.App;
using AndroidX.Fragment;
using Fragment = AndroidX.Fragment.App.Fragment;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;
using AndroidX.AppCompat.App;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
using System.Net.Http;
using Newtonsoft.Json;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using Android.Util;
using System.Threading;

namespace HOTAYI_MWMS
{
    [Activity(Label = "RequestActivity")]
    public class RequestActivity : AppCompatActivity, EMDKManager.IEMDKListener
    {
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager manager;
        private RequestAdapter adapter;
        private List<ProdRequest> requests;
        private ImageButton btn_addR, btn_update;

        // Declare a variable to store EMDKManager object
        private EMDKManager emdkManager = null;
        // Declare a variable to store BarcodeManager object
        private BarcodeManager barcodeManager = null;
        // Declare a variable to store Scanner object
        private Scanner scanner = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_request);
            SupportActionBar.Title = "Production Request";
            // Set up EMDK to get the scanner ready
            EMDKResults results = EMDKManager.GetEMDKManager(Application.Context, this);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.rc_request);
            manager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(manager);

            btn_addR = FindViewById<ImageButton>(Resource.Id.btn_addR);
            btn_addR.Click += delegate
            {
                // add request
                FragmentTransaction fragmentT= SupportFragmentManager.BeginTransaction();
                AddRequestFragment addRequest = new AddRequestFragment();
                addRequest.Show(fragmentT, "DialogFragment");
            };

            btn_update = FindViewById<ImageButton>(Resource.Id.btn_updateM);
            btn_update.Click += delegate
            {
                // pass the request to backend to get rack location
                if(requests!= null)
                {
                    processRequest();
                }
                //Finish();
            };
        }

        public async void addRequest(string partN, int qty)
        {
            if (requests == null)
            {
                requests = new List<ProdRequest>();
            }

            int index = 99;
            bool newItem = true;
            for(int i=0;i < requests.Count;i++)
            {
                if(partN == requests[i].partNum)
                {
                    HttpClient client = new HttpClient();
                    int total = requests[i].qty_request + qty;
                    string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/ValidQTY?partN=" + partN + "&qty=" + total;
                    var uri = new Uri(url);
                    HttpResponseMessage responseMessage = await client.GetAsync(uri);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        string content = await responseMessage.Content.ReadAsStringAsync();
                        if (content == "PASS")
                        {
                            index = i;
                            newItem = false;
                            requests[i].qty_request += qty;
                        }
                        else
                        {
                            Toast.MakeText(Application.Context, "Part number or Quantity is invalid", ToastLength.Short).Show();
                            return;
                        }
                    }
                    break;
                }
            }
            if (newItem)
            {
                var req = new ProdRequest();
                req.partNum = partN;
                req.qty_request = qty;
                HttpClient client = new HttpClient();
                string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/ValidQTY?partN=" + partN + "&qty=" + qty;
                var uri = new Uri(url);
                HttpResponseMessage responseMessage = await client.GetAsync(uri);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string content = await responseMessage.Content.ReadAsStringAsync();
                    if (content == "PASS")
                    {
                        requests.Add(req);
                    }
                    else
                    {
                        Toast.MakeText(Application.Context, "Part number or Quantity is invalid", ToastLength.Short).Show();
                        return;
                    }
                }
                //requests.Sort();
            }

            if (adapter == null)
            {
                adapter = new RequestAdapter(requests);
                recyclerView.SetAdapter(adapter);
            }
            else
            {
                if (!newItem && index != 99)
                    adapter.NotifyItemChanged(index);
                else if (newItem)
                    adapter.NotifyDataSetChanged();
            }
        }

        public async void processRequest()
        {
            HttpClient client = new HttpClient();
            string parts = "";
            string eachQ = "";
            foreach(var r in requests)
            {
                parts += r.partNum + ",";
                eachQ += r.qty_request + ",";
            }
            parts = parts.Substring(0, parts.Length - 1);
            eachQ = eachQ.Substring(0, eachQ.Length - 1);
            string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/ProcessRequests?partNs=" + parts + "&eachQty=" + eachQ;
            var uri = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(uri);
            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                Intent intent = new Intent(this, typeof(MapActivity));
                intent.PutExtra("DestRacks", content);
                SetResult(Result.Ok, intent);
                this.Finish();
            }
        }

        public void OnClosed()
        {
            if (emdkManager != null)
            {
                if (barcodeManager != null)
                {
                    barcodeManager = null;
                }

                // Release all the resources
                emdkManager.Release();
                emdkManager = null;
            }
        }

        public void OnOpened(EMDKManager p0)
        {
            emdkManager = p0;
            //throw new System.NotImplementedException();
            InitScanner();
            scanner.Read();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // De-initialize scanner
            DeinitScanner();

            // Clean up the objects created by EMDK manager
            if (barcodeManager != null)
            {
                barcodeManager = null;
            }

            if (emdkManager != null)
            {
                emdkManager.Release();
                emdkManager = null;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            // The application is in background

            // De-initialize scanner
            DeinitScanner();

            if (barcodeManager != null)
            {
                // Remove connection listener
                barcodeManager = null;
            }

            // Release the barcode manager resources
            if (emdkManager != null)
            {
                emdkManager.Release(EMDKManager.FEATURE_TYPE.Barcode);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            // The application is in foreground 

            // Acquire the barcode manager resources
            if (emdkManager != null)
            {
                try
                {
                    barcodeManager = (BarcodeManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.StackTrace);
                }
            }
        }

        public void InitScanner()
        {
            if (emdkManager != null)
            {
                if (barcodeManager == null)
                {
                    try
                    {
                        //Get the feature object such as BarcodeManager object for accessing the feature.
                        barcodeManager = (BarcodeManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);

                        scanner = barcodeManager.GetDevice(BarcodeManager.DeviceIdentifier.Default);

                        if (scanner != null)
                        {
                            scanner.Enable();
                            //Attahch the Data Event handler to get the data callbacks.
                            scanner.Data += scanner_Data;
                            //Attach Scanner Status Event to get the status callbacks.
                            scanner.Status += scanner_Status;

                            //EMDK: Configure the scanner settings
                            ScannerConfig config = scanner.GetConfig();
                            config.SkipOnUnsupported = ScannerConfig.SkipOnUnSupported.None;
                            config.ScanParams.DecodeLEDFeedback = true;
                            config.DecoderParams.Ean8.Enabled = true;
                            config.DecoderParams.Ean13.Enabled = true;
                            config.DecoderParams.Code39.Enabled = true;
                            config.DecoderParams.Code128.Enabled = true;

                            scanner.SetConfig(config);

                            //set the scanning after trigger is pressed
                            scanner.TriggerType = Scanner.TriggerTypes.Hard;

                        }
                        else
                        {
                            Console.WriteLine("Error: Failed to enable scanner");
                        }
                    }
                    catch (ScannerException e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }

        public void DeinitScanner()
        {
            if (emdkManager != null)
            {

                if (scanner != null)
                {
                    try
                    {
                        scanner.CancelRead();
                        scanner.Disable();
                    }
                    catch (ScannerException e)
                    {
                        Log.Debug(this.Class.SimpleName, "Exception:" + e.Result.Description);
                    }

                    scanner.Data -= scanner_Data;
                    scanner.Status -= scanner_Status;

                    try
                    {
                        // Release the scanner
                        scanner.Release();
                    }
                    catch (ScannerException e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }

                }

                if (barcodeManager != null)
                {
                    emdkManager.Release(EMDKManager.FEATURE_TYPE.Barcode);
                }

                barcodeManager = null;
                scanner = null;
            }
        }

        public void scanner_Data(object sender, Scanner.DataEventArgs e)
        {
            ScanDataCollection scanDataCollection = e.P0;

            if ((scanDataCollection != null) && (scanDataCollection.Result == ScannerResults.Success))
            {
                IList<ScanDataCollection.ScanData> scanData = scanDataCollection.GetScanData();

                foreach (ScanDataCollection.ScanData data in scanData)
                {
                    string dataString = data.Data;

                    // Do something on the scanned result
                    var f = SupportFragmentManager.FindFragmentByTag("DialogFragment") as AddRequestFragment;
                    RunOnUiThread(() =>
                    {
                        f.setInput(dataString);
                    });
                }
            }
        }

        public void scanner_Status(object sender, Scanner.StatusEventArgs e)
        {
            StatusData statusData = e.P0;
            StatusData.ScannerStates state = e.P0.State;

            if (state == StatusData.ScannerStates.Idle)
            {
                // for continuous reading of barcode
                try
                {
                    // An attempt to use the scanner continuously and rapidly (with a delay < 100 ms between scans) 
                    // may cause the scanner to pause momentarily before resuming the scanning. 
                    // Hence add some delay (>= 100ms) before submitting the next read.
                    try
                    {
                        Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }

                    // Submit another read to keep the continuation
                    scanner.Read();
                }
                catch (ScannerException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

        }
    }
}