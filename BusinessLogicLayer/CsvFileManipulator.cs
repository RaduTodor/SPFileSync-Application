namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Text;
    using CsvHelper;
    using Models;
    using Common.Constants;
    using DataAccessLayer;

    //TODO [CR RT]: Add class and methods documentation

    public static class CsvFileManipulator
    {
        public static void WriteMetadata<T>(string filePath, List<T> list)
        {
            using (var csv = new CsvWriter(File.CreateText(filePath)))
            {
                csv.Configuration.Delimiter = HelpersConstant.CsvDelimiter;
                csv.WriteRecords(list);
            }
        }

        public static List<T> ReadMetadata<T>(string filePath, BaseListReferenceProvider listReferenceProvider)
        {
            CreateAccesibleFile(filePath, listReferenceProvider);
            var records = new List<T>();
            using (var file = File.OpenText(filePath))
            {
                using (var csv = new CsvReader(file))
                {
                    csv.Configuration.RegisterClassMap<MetadataModelCsvMap>();
                    csv.Configuration.Delimiter = HelpersConstant.CsvDelimiter;
                    csv.Configuration.Encoding = Encoding.UTF8;
                    try
                    {
                        records = csv.GetRecords<T>().ToList();
                    }
                    catch (Exception ex)
                    {
                        //TODO [CR RT]: Log exception
                    }
                }
            }

            return records;
        }

        public static void CreateAccesibleFile(string filePath, BaseListReferenceProvider listReferenceProvider)
        {
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(listReferenceProvider.ConnectionConfiguration.DirectoryPath);
                var info = new DirectoryInfo(listReferenceProvider.ConnectionConfiguration.DirectoryPath);
                var security = info.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(WindowsIdentity.GetCurrent().Name,
                    FileSystemRights.Modify, InheritanceFlags.ContainerInherit, PropagationFlags.None,
                    AccessControlType.Allow));
                security.AddAccessRule(new FileSystemAccessRule(WindowsIdentity.GetCurrent().Name,
                    FileSystemRights.Modify, InheritanceFlags.ObjectInherit, PropagationFlags.None,
                    AccessControlType.Allow));
                info.SetAccessControl(security);

                var streamWriter = File.CreateText(filePath);
                streamWriter.Close();
            }
        }
    }
}