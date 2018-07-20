namespace Models
{
    using Common.Constants;

    public sealed class MetadataModelCsvMap : CsvHelper.Configuration.ClassMap<MetadataModel>
    {
        public MetadataModelCsvMap()
        {
            Map(m => m.Url).Name(HelpersConstants.CsvMetadataUrlFieldName);

            Map(m => m.ModifiedDate).Name(HelpersConstants.CsvMetadataModifiedDateFieldName);
        }
    }
}
