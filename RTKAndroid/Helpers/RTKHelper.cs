using Android.App;
using Android.Content;
using Android.Locations;
using Android.Telephony;
using Com.QX.WZ.Sdk.Rtcm;
using System;
using System.IO;

namespace RTKAndroid.Helpers
{
    public class RTKHelper : Java.Lang.Object, IWzRtcmListener, GpsStatus.INmeaListener
    {
        private IWzRtcmManager _RtcmManager;
        private bool isStart;
        private Action<RtcmSnippet> action;
        private readonly Activity owner;

        private readonly string APP_KEY = "710700";
        private readonly string APP_SECRET = "90539a3850bb8fbd6d7c9342afdba9232ea3f75bfad4b8071642c8948dbf5a03";
        private readonly string DEVICE_TYPE = "RTK";
        private readonly string DEVICE_ID;
        private readonly string GGA = "$GPGGA,074459.91,2305.5104600,N,11348.9600000,E,1,00,1.0,4.915,M,-4.915,M,0.0,*5F";

        public RTKHelper(Activity owner)
        {
            this.owner = owner;
            DEVICE_ID = ((TelephonyManager)owner.GetSystemService(Context.TelephonyService)).DeviceId;
            isStart = false;
            _RtcmManager = WzRtcmFactory.GetWzRtcmManager(owner, APP_KEY, APP_SECRET, DEVICE_ID, DEVICE_TYPE, null);
        }

        public void Start(Action<RtcmSnippet> action)
        {
            if (isStart) return;
            isStart = true;
            this.action = action;
            _RtcmManager.RequestRtcmUpdate(this, 31.2065750000, 121.4684340000, null);
            HelperManger.LocationHelper.Start(this);
        }

        public void Stop()
        {
            if (!isStart) return;
            isStart = false;
            HelperManger.LocationHelper.Stop();
            _RtcmManager.RemoveUpdate(this);
        }

        public void OnRtcmDatachanged(RtcmSnippet p0)
        {
            action(p0);
        }

        public void OnStatusChanaged(int p0, string p1)
        {

        }

        public new void Dispose()
        {
            if (isStart)
            {
                _RtcmManager.RemoveUpdate(this);
                _RtcmManager.Close();
            }
            isStart = false;
        }

        public void OnNmeaReceived(long timestamp, string nmea)
        {
            if (nmea.Substring(1, 5) == "GPGGA")
            {
                _RtcmManager.SendGGA(nmea);
                File.AppendAllText(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/MySLAM/nmea.txt", nmea);
            }
        }
    }
}