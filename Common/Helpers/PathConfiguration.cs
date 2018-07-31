﻿namespace Common.Helpers
{
    using Common.Constants;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;

    public static class PathConfiguration
    {
        public static string SetPath(string defaultPath = "")
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = defaultPath;
            folder.ShowDialog();
            return folder.SelectedPath;
        }

        public static string GetApplicationDirectory(string wantedResource)
        {
            var path = Directory.GetCurrentDirectory();
            //TODO [CR BT]: Use culture info  path.IndexOf(ConfigurationMessages.Bin, StringComparison.Ordinal);
            var removeSegment = path.IndexOf(ConfigurationMessages.Bin);
            var resourceFolderPath = ($@"{path.Remove(removeSegment)}{wantedResource}").ToString(CultureInfo.InvariantCulture);
            return resourceFolderPath;
        }
    }
}
