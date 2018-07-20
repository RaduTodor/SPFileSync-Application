namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CsvHelper;
    using Models;
    using Common.Constants;
    using Common.Helpers;
    using Common.Exceptions;


    /// <summary>
    /// This Class writes and reads Metadata needed for Synchronization check. It uses CsvHelper
    /// </summary>
    public static class CsvMetadataFileManipulator
    {
        /// <summary>
        /// The writing method is generic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="list"></param>
        public static void WriteMetadata<T>(string filePath, List<T> list)
        {
            using (var csv = new CsvWriter(File.CreateText(filePath)))
            {
                csv.Configuration.Delimiter = HelpersConstants.CsvDelimiter;
                csv.WriteRecords(list);
            }
        }

        /// <summary>
        /// The reading method also is generic and needs a directoryPath 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static List<T> ReadMetadata<T>(string filePath, string directoryPath)
        {
            var records = new List<T>();
            try
            {
                FileEditingHelper.CreateAccesibleFile(filePath, directoryPath);
                using (var file = File.OpenText(filePath))
                {
                    using (var csv = new CsvReader(file))
                    {
                        csv.Configuration.RegisterClassMap<MetadataModelCsvMap>();
                        csv.Configuration.Delimiter = HelpersConstants.CsvDelimiter;
                        csv.Configuration.Encoding = Encoding.UTF8;
                        try
                        {
                            records = csv.GetRecords<T>().ToList();
                        }
                        catch (Exception exception)
                        {
                            throw new MetadataReadException(DefaultExceptionMessages.ErrorMetadataReadExceptionMessage, exception);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return records;
        }
    }
}