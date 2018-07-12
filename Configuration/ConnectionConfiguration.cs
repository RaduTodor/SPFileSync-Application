using System;
using System.Collections.Generic;

namespace Configuration
{
    public class ConnectionConfiguration
    {
        public Connection Connection { get; set; }

        public Dictionary<string,List<string>> ListsAndColumns { get; set; }

        public string DirectoryPath { get; set; }

        public TimeSpan SyncTimeSpan { get; set; }
    }
}
