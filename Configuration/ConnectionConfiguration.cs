using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Xml.Serialization;

namespace Configuration
{
    //TODO [CR RT]: Extract constants -> create new class library project called Commom. Keep constant values there. Add reference to Common for each project
    //TODO [CR RT]: Remove unused members
    //TODO [CR RT]: Add class and methods documentation

    public class ConnectionConfiguration
    {
        public Connection Connection { get; set; }

        public List<ListWithColumnsName> ListsWithColumnsNames { get; set; }

        //TODO [CR RT] : Use Path.Combine(p1, p2); instead.
        //TODO [CR RT] : Initialize properties from another place or make it constant
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
