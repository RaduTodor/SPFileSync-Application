using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Configuration
{
    //TODO [CR RT]: Should be moved to Common, in a Helper class
    //TODO [CR RT]: To be made static
    //TODO [CR RT]: Add class and methods documentation
    //TODO [CR RT]: Remove unused using, move under namespace
    //TODO [CR RT]: Extarct constants
    //TODO [CR RT]: To be checked if there is code duplicate with the one from CsvFileManipulator class

    public class ConnectionConfigurationXmlManipulator
    {
        public static void Serialize(List<ConnectionConfiguration> configurations)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(configurations.GetType());
            Directory.GetCurrentDirectory();
            if (!System.IO.File.Exists(Directory.GetCurrentDirectory() + @"\Data\configurations.xml"))
            {
                System.IO.Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Data");
                DirectoryInfo info = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Data");
                DirectorySecurity security = info.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                    FileSystemRights.Modify, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                security.AddAccessRule(new FileSystemAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                    FileSystemRights.Modify, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                info.SetAccessControl(security);

                StreamWriter streamWriter = System.IO.File.CreateText(Directory.GetCurrentDirectory() + @"\Data\configurations.xml");
                streamWriter.Close();
            }
            TextWriter textWriter = File.CreateText(Directory.GetCurrentDirectory() + @"\Data\configurations.xml");
            xmlSerializer.Serialize(textWriter, configurations);
        }
        public static List<ConnectionConfiguration> Deserialize()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(new List<ConnectionConfiguration>().GetType());
            TextReader textReader = File.OpenText(Directory.GetCurrentDirectory() + @"\Data\configurations.xml");

            return (List<ConnectionConfiguration>)xmlSerializer.Deserialize(textReader);

        }
    }
}
