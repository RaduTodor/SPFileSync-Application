using CsvHelper;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace BusinessLogicLayer
{
    public class CsvFileManipulator
    {
        public static void WriteMetadata<T>(string filePath, List<T> list)
        {
            using (var csv = new CsvWriter(System.IO.File.CreateText(filePath)))
            {
                csv.Configuration.Delimiter = ";";
                csv.WriteRecords(list);
            }
        }

        public static List<T> ReadMetadata<T>(string filePath, DataAccessOperations dataAccessOperations)
        {
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(dataAccessOperations.ConnectionConfiguration.DirectoryPath);
                DirectoryInfo info = new DirectoryInfo(dataAccessOperations.ConnectionConfiguration.DirectoryPath);
                DirectorySecurity security = info.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                    FileSystemRights.Modify, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                security.AddAccessRule(new FileSystemAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                    FileSystemRights.Modify, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                info.SetAccessControl(security);

                StreamWriter streamWriter = System.IO.File.CreateText(filePath);
                streamWriter.Close();
            }
            List<T> records = new List<T>();
            using (var file = System.IO.File.OpenText(filePath))
            {
                using (var csv = new CsvReader(file))
                {
                    csv.Configuration.RegisterClassMap<MetadataModelCsvMap>();
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.Encoding = Encoding.UTF8;
                    try
                    {
                        records = csv.GetRecords<T>().ToList();
                    }
                    catch (Exception ex)
                    {
                        if (ex is NullReferenceException)
                        {
                            records = new List<T>();
                        }
                    }
                }
            }
            return records;
        }
    }
}
