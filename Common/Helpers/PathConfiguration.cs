namespace Common.Helpers
{
    using System.Windows.Forms;

    public class PathConfiguration
    {
        public static string SetPath(string defaultPath = "")
        {
            var folder = new FolderBrowserDialog();
            folder.SelectedPath = defaultPath;
            folder.ShowDialog();
            return folder.SelectedPath;
        }
    }
}