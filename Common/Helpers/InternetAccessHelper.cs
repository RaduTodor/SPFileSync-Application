namespace Common.Helpers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Constants;

    public static class InternetAccessHelper
    {
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool HasInternetAccess()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }

        public static bool HasInternetAccessAfterRetryInterval()
        {
            Thread.Sleep(new TimeSpan(0,DataAccessLayerConstants.SyncRetryInterval,0));
            int description;
            return InternetGetConnectedState(out description, 0);
        }
    }
}
