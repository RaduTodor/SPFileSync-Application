namespace Models
{
    using Common.Constants;
    using CsvHelper.Configuration;

    public sealed class MetadataModelCsvMap : ClassMap<MetadataModel>
    {
        public MetadataModelCsvMap()
        {
            Map(m => m.Url).Name(HelpersConstants.CsvMetadataUrlFieldName);

            Map(m => m.ModifiedDate).Name(HelpersConstants.CsvMetadataModifiedDateFieldName);
        }
    }
}