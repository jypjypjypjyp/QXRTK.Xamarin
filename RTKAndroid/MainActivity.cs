using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using RTKAndroid.Helpers;
using System.IO;
using Com.QX.WZ.Sdk.Rtcm;

namespace RTKAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button startBtn, stopBtn;
        private TextView status;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            HelperManger.MainActivity = this;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            startBtn = FindViewById<Button>(Resource.Id.start_btn);
            stopBtn = FindViewById<Button>(Resource.Id.stop_btn);

            startBtn.Click += StartBtn_Click;
            stopBtn.Click += StopBtn_Click;
        }

        private void StopBtn_Click(object sender, System.EventArgs e)
        {
            HelperManger.RTKHelper.Stop();
        }

        private void StartBtn_Click(object sender, System.EventArgs e)
        {
            HelperManger.RTKHelper.Start(WriteData);
        }

        private void WriteData(RtcmSnippet rtcm)
        {
            using(var fs = new FileStream(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/MySLAM/rtcm.dat", FileMode.Append))
            {
                fs.Write(rtcm.GetBuffer(), 0, rtcm.GetBuffer().Length);
            }
            RunOnUiThread(() =>
            {
                status.Text = "Ok";
            });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}