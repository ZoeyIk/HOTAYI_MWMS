using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Android.Material.TextField;
using System.Net.Http;
using System.Net.Http.Headers;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using Android.Util;
using System.Threading;

namespace HOTAYI_MWMS
{
    [Activity(Label = "ReportActivity")]
    public class ReportActivity : Activity, View.IOnClickListener, EMDKManager.IEMDKListener
    {
        private TextInputLayout inputLayout;
        private TextInputEditText textInput_partNum;
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager manager;
        private List<ReelInfo> reelInfo;
        private ReportAdapter adapter;

        // Declare a variable to store EMDKManager object
        private EMDKManager emdkManager = null;
        // Declare a variable to store BarcodeManager object
        private BarcodeManager barcodeManager = null;
        // Declare a variable to store Scanner object
        private Scanner scanner = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ISharedPreferences pref2 = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            string data = pref2.GetString("Reels", String.Empty);
            //reelInfo = JsonConvert.DeserializeObject<List<ReelInfo>>(data);

            // Create your application here
            SetContentView(Resource.Layout.activity_report);

            // Set up EMDK to get the scanner ready
            EMDKResults results = EMDKManager.GetEMDKManager(Application.Context, this);

            inputLayout = FindViewById<TextInputLayout>(Resource.Id.textInputLayout3);
            textInput_partNum = FindViewById<TextInputEditText>(Resource.Id.input_partNum);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.rc_table);

            inputLayout.SetEndIconOnClickListener(this);

            textInput_partNum.TextChanged += TextInput_partNum_TextChanged;
        }

        private void TextInput_partNum_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if(inputLayout.Error != "" || inputLayout.Error != null)
            {
                inputLayout.Error = null;
            }
        }

        public async void OnClick(View v)
        {
            var partNum = textInput_partNum.Text;

            if (partNum == null || partNum == "")
            {
                inputLayout.Error = "Please enter a part number";
            }
            else
            {
                //bind data
                HttpClient client = new HttpClient();
                string url = $"https://hotayi-backend.azurewebsites.net/api/Reel/QueryPartNum?partN=" + partNum;
                var uri = new Uri(url);
                HttpResponseMessage responseMessage = await client.GetAsync(uri);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string content = await responseMessage.Content.ReadAsStringAsync();
                    reelInfo = JsonConvert.DeserializeObject<List<ReelInfo>>(content);
                }

                if(reelInfo.Count == 0)
                {
                    inputLayout.Error = "Please enter a valid part number";
                }
                else
                {
                    //set layout manager
                    manager = new LinearLayoutManager(this);
                    recyclerView.SetLayoutManager(manager);

                    //set adapter here
                    adapter = new ReportAdapter(reelInfo, partNum);
                    recyclerView.SetAdapter(adapter);
                }
                
            }

            //throw new NotImplementedException();
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
                    inputLayout.Error = "Error: " + e.Message;
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
                            inputLayout.Error = "Failed to enable scanner";
                        }
                    }
                    catch (ScannerException e)
                    {
                        inputLayout.Error = "Error: " + e.Message;
                    }
                    catch (Exception ex)
                    {
                        inputLayout.Error = "Error: " + ex.Message;
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
                    RunOnUiThread(() =>
                    {
                        textInput_partNum.Text = dataString;
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
                    RunOnUiThread(() =>
                    {
                        inputLayout.Error = "Error: " + ex.Message;
                    });
                    Console.WriteLine(ex.StackTrace);
                }
                catch (NullReferenceException ex)
                {
                    RunOnUiThread(() =>
                    {
                        inputLayout.Error = "Error: " + ex.Message;
                    });
                    Console.WriteLine(ex.StackTrace);
                }
            }

        }
    }
}