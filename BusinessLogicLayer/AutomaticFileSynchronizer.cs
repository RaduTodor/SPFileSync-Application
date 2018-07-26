namespace BusinessLogicLayer
{
    using Common.Helpers;
    using Configuration;
    using System;

    //TODO [CR RT] Remove unnecessary empty lines
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


        //TODO [CR RT] Extract to constants
        public void AutomaticSynchronize()
        {
            if (InternetAccessHelper.HasInternetAccess())
            {
                InternetAccessInformation?.Invoke(this, "The connection is reestablished");           
            }
        }


    }
}
