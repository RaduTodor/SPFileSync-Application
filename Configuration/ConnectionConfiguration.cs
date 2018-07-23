namespace Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
    using System.Xml.Serialization;
    using Models;

    /// <summary>
    ///     An instance of ConnectionConfiguration has everything needed for application's sharepoint manipulator methods need
    /// </summary>
    public class ConnectionConfiguration
    {
        public Connection Connection { get; set; }

        public List<ListWithColumnsName> ListsWithColumnsNames { get; set; }

        public string DirectoryPath { get; set; } = Path.Combine(ConfigurationManager.AppSettings["DirectoryPath"],
            DateTime.Now.Day.ToString());

        [XmlIgnore] public TimeSpan SyncTimeSpan { get; set; } = new TimeSpan(0, 10, 0);

        [XmlAttribute("syncTimeSpan")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long SynctimeSpanTicks
        {
            get => SyncTimeSpan.Ticks;
            set => SyncTimeSpan = new TimeSpan(value);
        }
    }
}