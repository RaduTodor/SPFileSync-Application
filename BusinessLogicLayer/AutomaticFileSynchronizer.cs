namespace BusinessLogicLayer
{
    using Common.Helpers;
    using Configuration;
    using System;


    public class AutomaticFileSynchronizer
    {
        public AutomaticFileSynchronizer()
        {               
        }

        public AutomaticFileSynchronizer(ConnectionConfiguration configuration)
        {
            ConnectionConfiguration = configuration;
        }

        public event EventHandler<string> InternetAccessInformation;



        private ConnectionConfiguration ConnectionConfiguration;

        public void AutomaticSynchronize()
        {
            if (InternetAccessHelper.HasInternetAccess())
            {
                InternetAccessInformation?.Invoke(this, "The connection is reestablished");           
            }
        }


    }
}
