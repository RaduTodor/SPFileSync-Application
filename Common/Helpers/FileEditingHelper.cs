namespace Common.Helpers
{
    using System.IO;
    using System.Security.AccessControl;
    using System.Security.Principal;
    /// <summary>
    /// Useful methods for File interaction
    /// </summary>
    public static class FileEditingHelper
    {
        /// <summary>
        /// Makes sure that a file (by filepath) it's existing and grants access to it's directory
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="DirectoryPath"></param>
        public static void CreateAccesibleFile(string filePath,string DirectoryPath)
        {
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(DirectoryPath);
                var info = new DirectoryInfo(DirectoryPath);
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
