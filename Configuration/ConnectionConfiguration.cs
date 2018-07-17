using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Xml.Serialization;

namespace Configuration
{
    public class ConnectionConfiguration
    {
        public Connection Connection { get; set; }

        public List<ListWithColumnsName> ListsWithColumnsNames { get; set; }

        public string DirectoryPath { get; set; } = $"{ConfigurationManager.AppSettings[("DirectoryPath")]}\\{DateTime.Now.Day}";

        [XmlIgnore]
        public TimeSpan SyncTimeSpan { get; set; } = new TimeSpan(0, 10, 0);

        [XmlAttribute("syncTimeSpan")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public long SynctimeSpanTicks
        {
            get { return SyncTimeSpan.Ticks; }
            set { SyncTimeSpan = new TimeSpan(value); }
        }
    }
}
