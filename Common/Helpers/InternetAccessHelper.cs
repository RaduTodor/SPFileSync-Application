namespace Common.Helpers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Constants;

    public static class InternetAccessHelper
    {
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool HasInternetAccess()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }

        public static bool HasInternetAccessAfterRetryInterval()
        {
            bool response = false;
            while (!response)
            {
                Thread.Sleep(new TimeSpan(0, 0, DataAccessLayerConstants.SyncRetryInterval));
                int description;
                response = InternetGetConnectedState(out description, 0);
            }
            return true;
        }
    }
}
