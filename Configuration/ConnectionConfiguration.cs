using Models;
using System;
using System.Collections.Generic;

namespace Configuration
{
    public class ConnectionConfiguration
    {
        public Connection Connection { get; set; }

        public List<ListWithColumnsName> ListsWithColumnsNames { get; set; }

        public string DirectoryPath { get; set; }

        public TimeSpan SyncTimeSpan { get; set; }
    }
}
