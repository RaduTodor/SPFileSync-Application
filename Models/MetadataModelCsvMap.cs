namespace Models
{
    using Common.Constants;

    public sealed class MetadataModelCsvMap : CsvHelper.Configuration.ClassMap<MetadataModel>
    {
        public MetadataModelCsvMap()
        {
            Map(m => m.ModifiedDate).Name(HelpersConstant.CsvMetadataModifiedDateFieldName);

            Map(m => m.Url).Name(HelpersConstant.CsvMetadataUrlFieldName);
        }
    }
}
