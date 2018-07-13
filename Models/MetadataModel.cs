using System;
using System.Globalization;

namespace Models
{
    public class MetadataModel
    {
        public string Url { get; set; }

        public DateTime ModifiedDate { get; set; }
    }

    public class MetadataModelCsvMap : CsvHelper.Configuration.ClassMap<MetadataModel>
    {
        public MetadataModelCsvMap()
        {
            Map(m => m.ModifiedDate).Name("ModifiedDate");

            Map(m => m.Url).Name("Url");
        }
    }
}
