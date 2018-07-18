using System;

namespace Models
{
    //TODO [CR RT]: Put each class in a separate cs file
    //TODO [CR RT]: Extract constant

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
