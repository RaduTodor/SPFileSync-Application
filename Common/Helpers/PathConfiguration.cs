using System;
using System.Collections.Generic;
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
    }
}
