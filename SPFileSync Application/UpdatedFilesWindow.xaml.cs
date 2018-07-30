using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPFileSync_Application
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using Common.Constants;
    using Models;

    /// <summary>
    /// Interaction logic for UpdatedFilesWindow.xaml
    /// </summary>
    public partial class UpdatedFilesWindow : Window
    {
        private ObservableCollection<string> _filesDetails;
        private List<string> _files;
        public UpdatedFilesWindow()
        {
            InitializeComponent();
            PopulateComboBox();
            _files = new List<string>();
            UpdatedFilesModel updatedFilesModel = UpdatedFilesModel.Instance;
            for (int position = 0; position < updatedFilesModel.UpdatedFilesName.Count; position++)
            {
                _files.Add(updatedFilesModel.GetFileDetailsAt(position)); 
            }
            PopulateUpdatedFilesList();
            allUpdatedFiles.ItemsSource = _filesDetails;
            GoToButton.IsEnabled = false;
            allUpdatedFiles.SelectionChanged += (sender, args) => { GoToButton.IsEnabled = true; };
        }

        private void PopulateComboBox()
        {
            configComboBox.Items.Add(ConfigurationMessages.ComboBoxRest);
            configComboBox.Items.Add(ConfigurationMessages.ComboBoxCsom);
        }

        private void PopulateUpdatedFilesList()
        {
            _filesDetails = new ObservableCollection<string>();
            foreach (var item in _files)
            {
                _filesDetails.Add(item);
            }
        }

        private void GotToFile_OnClick(object sender, RoutedEventArgs e)
        {
            UpdatedFilesModel updatedFilesModel = UpdatedFilesModel.Instance;
            string filePath = updatedFilesModel.UpdatedFilesLocation[allUpdatedFiles.SelectedIndex];
            if (File.Exists(updatedFilesModel.UpdatedFilesLocation[allUpdatedFiles.SelectedIndex]))
            {
                Process.Start("explorer.exe", "/select, " + filePath);
            }
        }
    }
}
