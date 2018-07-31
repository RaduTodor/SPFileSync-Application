namespace SPFileSync_Application
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using Models;

    /// <summary>
    ///     Interaction logic for UpdatedFilesWindow.xaml
    /// </summary>
    public partial class UpdatedFilesWindow : Window
    {
        private readonly List<string> _files;
        private ObservableCollection<string> _filesDetails;
        private string _explorerApplication = "explorer.exe";
        private string _selectOption = "/select, ";

        public UpdatedFilesWindow()
        {
            InitializeComponent();
            _files = new List<string>();
            var updatedFilesSingleton = UpdatedFiles.Instance;
            for (var position = 0; position < updatedFilesSingleton.Files.Count; position++)
                _files.Add(updatedFilesSingleton.GetFileDetailsAt(position));
            PopulateUpdatedFilesList();
            allUpdatedFiles.ItemsSource = _filesDetails;
            GoToButton.IsEnabled = false;
            allUpdatedFiles.SelectionChanged += (sender, args) => { GoToButton.IsEnabled = true; };
        }

        private void PopulateUpdatedFilesList()
        {
            _filesDetails = new ObservableCollection<string>();
            foreach (var item in _files) _filesDetails.Add(item);
        }

        private void GotToFile_OnClick(object sender, RoutedEventArgs e)
        {
            var updatedFiles = UpdatedFiles.Instance;
            var filePath = updatedFiles.Files[allUpdatedFiles.SelectedIndex].FileLocation;
            if (File.Exists(filePath)) Process.Start(_explorerApplication, _selectOption + filePath);
        }
    }
}