using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using RTKAndroid.Helpers;
using System.IO;
using Com.QX.WZ.Sdk.Rtcm;
using Android;
using Android.Content.PM;

namespace RTKAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button startBtn, stopBtn;
        private TextView status, text;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            HelperManager.MainActivity = this;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            startBtn = FindViewById<Button>(Resource.Id.start_btn);
            stopBtn = FindViewById<Button>(Resource.Id.stop_btn);
            status = FindViewById<TextView>(Resource.Id.status);
            text = FindViewById<TextView>(Resource.Id.text);

            startBtn.Click += StartBtn_Click;
            stopBtn.Click += StopBtn_Click;

            if (HelperManager.PermissionHelper.ConfirmPermissions(
                new string[]{
                        Manifest.Permission.AccessCoarseLocation,
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.ReadPhoneState,
                        Manifest.Permission.Internet,
                        Manifest.Permission.AccessNetworkState,
                        Manifest.Permission.AccessWifiState,
                        Manifest.Permission.ChangeWifiState,
                        Manifest.Permission.ChangeNetworkState,
                        Manifest.Permission.WriteExternalStorage,
                        Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.ReceiveBootCompleted,
                    }, 1))
            {
                startBtn.Enabled = true;
            }
        }

        private void StopBtn_Click(object sender, System.EventArgs e)
        {
            startBtn.Enabled = true;
            stopBtn.Enabled = false;
            HelperManager.RTKHelper.Stop();
        }

        private void StartBtn_Click(object sender, System.EventArgs e)
        {
            startBtn.Enabled = false;
            stopBtn.Enabled = true;
            HelperManager.RTKHelper.Start(WriteData, DisplayStatus);
        }

        public void DisplayNmea(string nmea)
        {
            RunOnUiThread(
                () =>
                {
                    text.Text = nmea;
                });
        }

        private void DisplayStatus(int p0, string p1)
        {
            RunOnUiThread(() =>
            {
                status.Text = p0 + ":" + p1;
            });
        }

        private void WriteData(RtcmSnippet rtcm)
        {
            using (var fs = new FileStream(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/MySLAM/rtcm.dat", FileMode.Append))
            {
                var bs = rtcm.GetBuffer();
                fs.Write(rtcm.GetBuffer(), 0, rtcm.GetBuffer().Length);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode != 1)
            {
                return;
            }
            foreach (int i in grantResults)
            {
                if (i != (int)Permission.Granted)
                {
                    Finish();
                    return;
                }
            }
            startBtn.Enabled = true;
        }
    }
}
}