namespace Common.Helpers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Constants;

    public static class InternetAccessHelper
    {
        [DllImport("wininet.dll")]
        //TODO [CR RT] Change Description -> description; ReservedValue - > reservedValue
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

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
                Thread.Sleep(new TimeSpan(0, DataAccessLayerConstants.SyncRetryInterval, 0));
                int description;
                response = InternetGetConnectedState(out description, 0);
            }
            return true;
        }
    }
}
