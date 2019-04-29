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
        private Action<RtcmSnippet> dataChanged;
        private Action<int, string> statusChanged;
        private readonly MainActivity owner;

        private readonly string APP_KEY = "xxxxxxx";
        private readonly string APP_SECRET = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        private readonly string DEVICE_TYPE = "RTK";
        private readonly string DEVICE_ID;

        public RTKHelper(Activity owner)
        {
            this.owner = owner as MainActivity;
            DEVICE_ID = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            isStart = false;
            _RtcmManager = WzRtcmFactory.GetWzRtcmManager(owner, APP_KEY, APP_SECRET, DEVICE_ID, DEVICE_TYPE, null);
        }

        public void Start(Action<RtcmSnippet> dataAction, Action<int,string> statusAction = null)
        {
            if (isStart) return;
            isStart = true;
            this.dataChanged = dataAction;
            this.statusChanged = statusAction;
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
            dataChanged(p0);
        }

        public void OnStatusChanaged(int p0, string p1)
        {
            statusChanged?.Invoke(p0, p1);
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
            owner.DisplayNmea(nmea);
            if (nmea.Substring(1, 5) == "GPGGA")
            {
                _RtcmManager.SendGGA(nmea);
                File.AppendAllText(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/MySLAM/nmea.txt", nmea);
            }
        }
    }
}