using Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class ConnectionConfiguration
    {
        public Connection Connection { get; set; }

        public List<ListWithColumnsName> ListsWithColumnsNames { get; set; }

        public string DirectoryPath { get; set; } = $"{ConfigurationManager.AppSettings[("DirectoryPath")]}\\{Guid.NewGuid()}";

        public TimeSpan SyncTimeSpan { get; set; } = new TimeSpan(0, 10, 0);
    }
}
