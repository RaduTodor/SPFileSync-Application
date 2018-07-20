
namespace SPFileSync_Application
{
    using Configuration;
    using System.Windows;
    using BusinessLogicLayer;
    using Common.ApplicationEnums;
    using Common.Helpers;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var connectionConfigurations = XmlFileManipulator.Deserialize<ConnectionConfiguration>();
            FilesManager filesManager = new FilesManager(connectionConfigurations, ListReferenceProviderType.Rest);
            filesManager.Synchronize();
        }
    }
}
