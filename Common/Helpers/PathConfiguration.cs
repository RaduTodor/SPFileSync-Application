namespace Common.Helpers
{
    using Common.Constants;
    using System;
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
            var removeSegment = path.IndexOf(ConfigurationMessages.Bin,StringComparison.Ordinal);
            var resourceFolderPath = ($@"{path.Remove(removeSegment)}{wantedResource}").ToString(CultureInfo.InvariantCulture);
            return resourceFolderPath;
        }
    }
}
