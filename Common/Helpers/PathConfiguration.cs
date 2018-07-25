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
    //TODO [CR BT] Resolve usings
    //TODO [CR BT] Make class static

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
            //TODO [CR BT] Specify culture e.g. StringComparison.InvariantCultureIgnoreCase
            var removeSegment = path.IndexOf(ConfigurationMessages.Bin);
            var resourceFolderPath = $@"{path.Remove(removeSegment)}{wantedResource}";
            return resourceFolderPath;
        }
    }
}
