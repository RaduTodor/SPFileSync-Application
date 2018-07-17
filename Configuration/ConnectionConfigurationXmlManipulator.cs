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
