namespace Common.Helpers
{
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
            int TimeRemaining = DataAccessLayerConstants.SyncRetryInterval;
            while (TimeRemaining != 0)
            {
                Thread.Sleep(1000);
                TimeRemaining--;
            }
            
            int description;
            return InternetGetConnectedState(out description, 0);
        }
    }
}
