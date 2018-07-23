﻿using System;
using Common.Helpers;

namespace BusinessLogicLayer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Configuration;
    using Common.ApplicationEnums;

    /// <summary>
    /// An instance of FilesManager class can start the sync operations (check and download)
    /// </summary>
    public class FilesManager
    {
        //TODO [CR RT] : Use capital letters for properties connectionConfigurations - > ConnectionConfigurations
        private List<ConnectionConfiguration> connectionConfigurations { get; }

        private ListReferenceProviderType providerType { get; }

        public FilesManager(List<ConnectionConfiguration> configurations, ListReferenceProviderType type)
        {
            connectionConfigurations = configurations;
            providerType = type;
        }

        /// <summary>
        /// Synchronize method iterates all ConnectionConfiguration in connectionConfigurations and creates a new Task
        /// which calls and runs a FileSynchronizer instance Synchronize method.
        /// This is basically the Application Synchronization start.
        /// </summary>
        public void Synchronize()
        {
            foreach (var connection in connectionConfigurations)
            {
                try
                {
                    var fileSync = new FileSynchronizer(connection, providerType);
                    Task.Run(() => fileSync.Synchronize());
                }
                catch (Exception exception)
                {
                    MyLogger.Logger.Error(exception,exception.Message);
                }
            }
        }
    }
}
