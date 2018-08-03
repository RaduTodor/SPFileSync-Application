namespace Common.Helpers
{
    using System;
    using System.IO;
    using Constants;
    using IWshRuntimeLibrary;
    using Microsoft.WindowsAPICodePack.Shell;
    using File = System.IO.File;

    public static class ShortcutsHelper
    {
        private const string Slash = "/";
        private const string Backslash = "\\";
        private const string Space = " ";
        private const string CodedSpace = "%20";
        private const string ShortcutApplicationArgument = "-k";
        private const string ShortcutEmptyArgument = "";

        //TODO CR: Too many parameters. Please consider to extract a class with these parameters and use the new class as parameter also in ConstructShortcut method
        public static void CreateFileShortcut(string fileName, string filePath, string fileDirectoryPath,
            string arguments)
        {
            try
            {
                if (arguments == ShortcutEmptyArgument)
                {
                    IWshShortcut shortcut = ConstructShortcut(HelpersConstants.FileShortcutLocation,
                        HelpersConstants.FileShortcutsDirectory, fileName, filePath, arguments);
                    shortcut.WorkingDirectory = fileDirectoryPath;
                    shortcut.Save();
                    AddShortcutDirectoryInLibrary(HelpersConstants.FileShortcutsLibraryName, HelpersConstants.FileShortcutsDirectory);
                }
                else
                {

                    IWshShortcut shortcut = ConstructShortcut(HelpersConstants.SharepointFileShortcutLocation,
                        HelpersConstants.SharepointFileShortcutsDirectory, fileName, filePath, arguments);
                    shortcut.Save();
                    AddShortcutDirectoryInLibrary(HelpersConstants.SharepointFileShortcutsLibraryName, HelpersConstants.SharepointFileShortcutsDirectory);
                }
            }

            catch (Exception exception)
            {
                LoggerManager.Logger.Error(exception, exception.Message);
            }
        }

        //TODO CR: Too many parameters. 
        private static IWshShortcut ConstructShortcut(string shortcutLocation, string shortcutDirectory, string fileName, string filePath, string arguments)
        {
            if (!File.Exists(string.Format(shortcutLocation, fileName)))
            {
                //TODO CR: Please extract a variable and do not use + when the path is created. Plaese use string format or url utility classes.
                FileEditingHelper.CreateAccesibleFile(
                    Directory.GetCurrentDirectory() +
                    string.Format(shortcutLocation, fileName),
                    Directory.GetCurrentDirectory() + shortcutDirectory);

                var wsh = new WshShellClass();
                //TODO CR: Please extract a variable and do not use + when the path is created. Plaese use string format or url utility classes.
                var shortcut = wsh.CreateShortcut(
                    Directory.GetCurrentDirectory() +
                    string.Format(shortcutLocation, fileName)) as IWshShortcut;
                if (shortcut != null)
                {
                    shortcut.Arguments = arguments;
                    shortcut.TargetPath = filePath;
                    shortcut.WindowStyle = 1;
                    shortcut.Description = string.Format(HelpersConstants.ShortcutDescription, fileName);
                    return shortcut;
                }
            }

            return null;
        }

        private static void AddShortcutDirectoryInLibrary(string libraryName, string shortcutDirectory)
        {
            CreateLibrary(libraryName); 
            //TODO CR: Please extract a variable and do not use + when the path is created. Plaese use string format or url utility classes.
            AddFolderToLibrary(libraryName,
                Directory.GetCurrentDirectory() + shortcutDirectory);
        }

        private static void AddFolderToLibrary(string libraryName, string folderPath)
        {
            var shellLibrary = ShellLibrary.Load(libraryName, false);
            var shortcutsFolder =
                ShellFileSystemFolder.FromFolderPath(folderPath);
            if (!shellLibrary.Contains(shortcutsFolder)) shellLibrary.Add(shortcutsFolder);
        }

        private static void CreateLibrary(string libraryName)
        {
            try
            {
                 new ShellLibrary(libraryName, HelpersConstants.WindowsLibrariesLocation, false);
            }
            catch (Exception exception)
            {
                LoggerManager.Logger.Error(exception, exception.Message);
            }
        }

        public static void CreateUrlShortcut(string fileName, string filePath, string fileDirectoryPath)
        {
            filePath = AddWebDavToTargetPath(filePath);
            CreateFileShortcut(fileName, filePath, fileDirectoryPath, ShortcutApplicationArgument);
        }

        private static string AddWebDavToTargetPath(string filePath)
        {
            var uri = new Uri(filePath);
            var fileSharepointPath =
                string.Format(HelpersConstants.FileSharepointShortcutPath, uri.Authority, uri.LocalPath);
            filePath = fileSharepointPath.Replace(Slash, Backslash);
            return filePath.Replace(CodedSpace, Space);
        }
    }
}