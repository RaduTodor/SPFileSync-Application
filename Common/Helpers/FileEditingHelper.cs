namespace Common.Helpers
{
    using System;
    using System.IO;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using Constants;
    using Exceptions;

    /// <summary>
    ///     Useful methods for File interaction
    /// </summary>
    public static class FileEditingHelper
    {
        /// <summary>
        ///     Makes sure that a file (by filepath) it's existing and grants access to it's directory
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="directoryPath"></param>
        public static void CreateAccesibleFile(string filePath, string directoryPath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(directoryPath);
                    var info = new DirectoryInfo(directoryPath);
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
            catch (Exception exception)
            {
                Exception currentException =
                    new CreateFileException(DefaultExceptionMessages.CreateFileExceptionMessage, exception);
                LoggerManager.Logger.Error(currentException, currentException.Message);
                throw currentException;
            }
        }
    }
}