namespace Configuration
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.AccessControl;
    using System.Xml.Serialization;

    public class ConnectionConfigurationXmlManipulator
    {
        public static void Serialize(List<ConnectionConfiguration> configurations)
        {
            var xmlSerializer = new XmlSerializer(configurations.GetType());
            Directory.GetCurrentDirectory();
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\Data\configurations.xml"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Data");
                var info = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Data");
                var security = info.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                    FileSystemRights.Modify, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                security.AddAccessRule(new FileSystemAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                    FileSystemRights.Modify, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                info.SetAccessControl(security);

                var streamWriter = File.CreateText(Directory.GetCurrentDirectory() + @"\Data\configurations.xml");
                streamWriter.Close();
            }
            TextWriter textWriter = File.CreateText(Directory.GetCurrentDirectory() + @"\Data\configurations.xml");
            xmlSerializer.Serialize(textWriter, configurations);
        }
        public static List<ConnectionConfiguration> Deserialize()
        {
            var xmlSerializer = new XmlSerializer(new List<ConnectionConfiguration>().GetType());
            TextReader textReader = File.OpenText(Directory.GetCurrentDirectory() + @"\Data\configurations.xml");

            return (List<ConnectionConfiguration>)xmlSerializer.Deserialize(textReader);

        }
    }
}
