using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.BottomNavigation;
using AndroidX.Fragment;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using Android.Util;
using System.Collections.Generic;
using System;
using System.Threading;

namespace HOTAYI_MWMS
{
    [Activity(Label = "MainActivity")]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener, EMDKManager.IEMDKListener
    {
        // Declare fragments
        private Fragment recv_frag, str_frag, iss_frag;

        // Declare a variable to store EMDKManager object
        private EMDKManager emdkManager = null;

        // Declare a variable to store BarcodeManager object
        private BarcodeManager barcodeManager = null;

        // Declare a variable to store Scanner object
        private Scanner scanner = null;

        // Declare view items
        private TextView statusView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.activity_main);

            //textMessage = FindViewById<TextView>(Resource.Id.message);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            recv_frag = new ReceiveFragment();
            str_frag = new StoreFragment();
            iss_frag = new IssueFragment();

            // Let the receive item interface appears first
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frg_container, recv_frag, "RecvFrag").Commit();
            statusView = FindViewById<TextView>(Resource.Id.statusView);

            //enable emdk
            EMDKResults results = EMDKManager.GetEMDKManager(Application.Context, this);
            if (results.StatusCode != EMDKResults.STATUS_CODE.Success)
            {
                // EMDKManager object initialization failed
                displayStatus("Status: EMDKManager object creation failed.");
            }
            else
            {
                // EMDKManager object initialization succeeded
                displayStatus("Status: EMDKManager object creation succeeded.");
            }

        }
        
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_receive:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frg_container, recv_frag, "RecvFrag").Commit();
                    return true;
                case Resource.Id.navigation_store:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frg_container, str_frag, "StrFrag").Commit();
                    return true;
                case Resource.Id.navigation_issue:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frg_container, iss_frag, "IssFrag").Commit();
                    return true;
            }
            return false;
        }

        public void GetCurrentFrag(string data)
        {
            var f = SupportFragmentManager.FindFragmentById(Resource.Id.frg_container);
            switch (f.Tag)
            {
                case "RecvFrag":
                    var frag1 = f as ReceiveFragment;
                    RunOnUiThread(() => 
                    {
                        frag1.setInput(data);
                    });
                    break;
                case "StrFrag":
                    var frag2 = f as StoreFragment;
                    RunOnUiThread(() =>
                    {
                        frag2.setInput(data);
                    });
                    break;
                case "IssFrag":
                    var frag3 = f as IssueFragment;
                    RunOnUiThread(() =>
                    {
                        frag3.setInput(data);
                    });
                    break;
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
            //throw new System.NotImplementedException();
        }

        public void OnOpened(EMDKManager p0)
        {
            emdkManager = p0;
            displayStatus("Status: EMDK is opened successfully.");
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
                    displayStatus("Status: BarcodeManager object creation failed.");
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
                            displayStatus("Error: Failed to enable scanner. ");
                        }
                    }
                    catch (ScannerException e)
                    {
                        displayStatus("Error: " + e.Message);
                    }
                    catch (Exception ex)
                    {
                        displayStatus("Error: " + ex.Message);
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
                        displayStatus(e.Message);
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
                    GetCurrentFrag(dataString);
                }
            }
        }

        public void scanner_Status(object sender, Scanner.StatusEventArgs e)
        {
            StatusData statusData = e.P0;
            StatusData.ScannerStates state = e.P0.State;

            if (state == StatusData.ScannerStates.Idle)
            {
                RunOnUiThread(() =>
                {
                    displayStatus("Status: Scanner is idle and ready.");
                });

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
                        displayStatus("Error: " + ex.Message);
                    });
                    Console.WriteLine(ex.StackTrace);
                }
                catch (NullReferenceException ex)
                {
                    RunOnUiThread(() =>
                    {
                        displayStatus("Error: " + ex.Message);
                    });
                    Console.WriteLine(ex.StackTrace);
                }
            }

            if (state == StatusData.ScannerStates.Waiting)
            {
                RunOnUiThread(() =>
                {
                    displayStatus("Status: Scanner is waiting for trigger press.");
                });
            }

            if (state == StatusData.ScannerStates.Scanning)
            {
                RunOnUiThread(() =>
                {
                    displayStatus("Status: Scanning...");
                });

            }

            if (state == StatusData.ScannerStates.Disabled)
            {
                RunOnUiThread(() =>
                {
                    displayStatus(statusData.FriendlyName + " is disabled.");
                });
            }

            if (state == StatusData.ScannerStates.Error)
            {
                RunOnUiThread(() =>
                {
                    displayStatus("Error: An error has occurred.");
                });
            }
        }

        public void displayStatus(string text)
        {
            statusView.Text = text;
        }
    }
}

