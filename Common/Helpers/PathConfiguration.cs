using Common.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Common.Helpers
{
  public class PathConfiguration
    {
        public static string SetPath(string defaultPath="")
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = defaultPath;
            folder.ShowDialog();
            return folder.SelectedPath;
        }

        public static string GetResourcesFolder(string wantedResource)
        {
            var path = Directory.GetCurrentDirectory();
            var removeSegment = path.IndexOf(ConfigurationMessages.Bin);
            var resourceFolderPath = $@"{path.Remove(removeSegment)}{wantedResource}";
            return resourceFolderPath;
        }
    }
}
