namespace Configuration
{
    using System.IO;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Xml.Serialization;

    //TODO [CR RT]: Extract constants -> create new class library project called Commom. Keep constant values there. Add reference to Common for each project
    //TODO [CR RT]: Remove unused members
    //TODO [CR RT]: Add class and methods documentation

    public class ConnectionConfiguration
    {
        public Connection Connection { get; set; }

        public List<ListWithColumnsName> ListsWithColumnsNames { get; set; }

        //TODO [CR RT] : Initialize properties from another place or make it constant
        public string DirectoryPath { get; set; } = Path.Combine(ConfigurationManager.AppSettings[("DirectoryPath")],DateTime.Now.Day.ToString());

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
