using Android.App;

namespace RTKAndroid.Helpers
{
    public class HelperManger
    {
        public static Activity MainActivity
        {
            set
            {
                if (value == _mainActivity) return;
                _mainActivity = value;
                _permissionHelper = null;
            }
            get => _mainActivity;
        }
        
        public static PermissionHelper PermissionHelper
        {
            get
            {
                if (_permissionHelper == null)
                {
                    _permissionHelper = new PermissionHelper(_mainActivity);
                }
                return _permissionHelper;
            }
        }

        public static LocationHelper LocationHelper
        {
            get
            {
                if (_locationHelper == null)
                {
                    _locationHelper = new LocationHelper(_mainActivity);
                }
                return _locationHelper;
            }
        }

        public static RTKHelper RTKHelper
        {
            get
            {
                if (_RTKHelper == null)
                {
                    _RTKHelper = new RTKHelper(_mainActivity);
                }
                return _RTKHelper;
            }
        }

        private static Activity _mainActivity;
        private static PermissionHelper _permissionHelper;
        private static RTKHelper _RTKHelper;
        private static LocationHelper _locationHelper;
    }
}