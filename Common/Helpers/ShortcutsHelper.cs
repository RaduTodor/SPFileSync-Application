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

        public static void CreateFileShortcut(ShortcutInformations shortcutInformations)
        {
            try
            {
                if (shortcutInformations.ShortcutArguments == ShortcutEmptyArgument)
                {
                    shortcutInformations.ShortcutLocation = HelpersConstants.FileShortcutLocation;
                    shortcutInformations.ShortcutDirectory = HelpersConstants.FileShortcutsDirectory;
                    IWshShortcut shortcut = ConstructShortcut(shortcutInformations);
                    shortcut.WorkingDirectory = shortcutInformations.FileDirectoryPath;
                    shortcut.Save();
                    AddShortcutDirectoryInLibrary(HelpersConstants.FileShortcutsLibraryName, HelpersConstants.FileShortcutsDirectory);
                }
                else
                {
                    shortcutInformations.ShortcutLocation = HelpersConstants.SharepointFileShortcutLocation;
                    shortcutInformations.ShortcutDirectory = HelpersConstants.SharepointFileShortcutsDirectory;
                    IWshShortcut shortcut = ConstructShortcut(shortcutInformations);
                    shortcut.Save();
                    AddShortcutDirectoryInLibrary(HelpersConstants.SharepointFileShortcutsLibraryName, HelpersConstants.SharepointFileShortcutsDirectory);
                }
            }

            catch (Exception exception)
            {
                LoggerManager.Logger.Error(exception, exception.Message);
            }
        }

        private static IWshShortcut ConstructShortcut(ShortcutInformations shortcutInformations)
        {
            if (!File.Exists(string.Format(shortcutInformations.ShortcutLocation, shortcutInformations.FileName)))
            {
                string filePath =
                    $"{Directory.GetCurrentDirectory()}{string.Format(shortcutInformations.ShortcutLocation, shortcutInformations.FileName)}";
                string directoryPath = $"{Directory.GetCurrentDirectory()}{shortcutInformations.ShortcutDirectory}";
                FileEditingHelper.CreateAccesibleFile(filePath,directoryPath);

                var wsh = new WshShellClass();

                string pathLink =
                    $"{Directory.GetCurrentDirectory()}{string.Format(shortcutInformations.ShortcutLocation, shortcutInformations.FileName)}";
                var shortcut = wsh.CreateShortcut(pathLink) as IWshShortcut;
                if (shortcut != null)
                {
                    shortcut.Arguments = shortcutInformations.ShortcutArguments;
                    shortcut.TargetPath = shortcutInformations.FilePath;
                    shortcut.WindowStyle = 1;
                    shortcut.Description = string.Format(HelpersConstants.ShortcutDescription, shortcutInformations.FileName);
                    return shortcut;
                }
            }

            return null;
        }

        private static void AddShortcutDirectoryInLibrary(string libraryName, string shortcutDirectory)
        {
            CreateLibrary(libraryName);
            string folderPath = $"{Directory.GetCurrentDirectory()}{shortcutDirectory}";
            AddFolderToLibrary(libraryName,folderPath);
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
            ShortcutInformations shortcutInformations = new ShortcutInformations
            {
                FileName = fileName,
                FilePath = filePath,
                FileDirectoryPath = fileDirectoryPath,
                ShortcutArguments = ShortcutApplicationArgument
            };
            CreateFileShortcut(shortcutInformations);
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