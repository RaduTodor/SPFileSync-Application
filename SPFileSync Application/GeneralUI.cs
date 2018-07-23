using Common.Constants;
using Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SPFileSync_Application
{
    //TODO [CR BT] :  Single Responsability principal violated. Remove this class because it's doing a lot of different operations which can be moved to existing/new classes. Even the name of the class it's show us that a lot of different things are made here.
    public class GeneralUI
    {
        //Should i move it to Business Logic Layer?

        private Window window;
        private BusinessLogicLayer.ObservableHashSet<string> errors = new BusinessLogicLayer.ObservableHashSet<string>();
        public GeneralUI(Window window)
        {
            this.window = window;
        }

        public GeneralUI()
        {

        }
        //TODO [CR BT] : Methods should start with Capital letter. Move this into another class in Configuration DLL.
        public static void checkConfiguration(ConnectionConfiguration configuration)
        {
            if (configuration == null)
            {
                configuration = new ConnectionConfiguration();
            }
            if (configuration.ListsWithColumnsNames == null)
            {
                configuration.ListsWithColumnsNames = new List<ListWithColumnsName>();
            }
        }
        //TODO [CR BT] : Extract method into another class in Common. Check if there is no existing class where you can move this method.
        public static string GetResourcesFolder(string wantedResource)
        {
            var path = Directory.GetCurrentDirectory();
            //TODO [CR BT] :Extract constant
            var removeSegment = path.IndexOf("bin");
            var resourceFolderPath = $@"{path.Remove(removeSegment)}{wantedResource}";
            return resourceFolderPath;
        }
        //TODO [CR BT] : Extract this method into another class eg. ConfigurationValidator  which should be created.
        //TODO [CR BT] : Rename method to eg. ValidateField
        public bool FieldValidation(string data, System.Windows.Controls.Label displayError)
        {
            bool verifyData = true;
            if (string.IsNullOrEmpty(data))
            {
                verifyData = false;
                DisplayWarning(displayError, ConfigurationMessages.EmptyField);
            }
            return verifyData;
        }
        //TODO [CR BT] : Extract method into another class in Common class eg. NotifyUI created above.
        //TODO [CR BT] : Rename Interval -> interval
        public void DisplayWarning(System.Windows.Controls.Label label, string message, int Interval = 2000)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = Interval;
            label.Dispatcher.Invoke(new Action(() => label.Content = message));
            label.Dispatcher.Invoke(new Action(() => label.Visibility = Visibility.Visible));
            timer.Elapsed += (sender, eventHandler) =>
            {
                label.Dispatcher.Invoke(new Action(() => label.Visibility = Visibility.Hidden));
                timer.Stop();
            };
            timer.Start();
        }

        //TODO [CR BT] : Extract method into another class eg. NotifyUI which should be created.
        public void NotifyError(NotifyIcon notifyIcon, string notificationTitle, string notificationMessage)
        {
            notifyIcon.BalloonTipTitle = notificationTitle;
            notifyIcon.BalloonTipText = notificationMessage;
            var path = Directory.GetCurrentDirectory();
            var pathCombine = Path.Combine(GetResourcesFolder(ConfigurationMessages.ResourceFolderErrorIcon));
            notifyIcon.Icon = new Icon(pathCombine);
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(5000);
            notifyIcon.Click += NotifyIconClick;
            notifyIcon.Text = notificationMessage;
        }
        //TODO [CR BT] : Extract method into another class eg. NotifyUI which should be created.
        private void NotifyIconClick(object sender, EventArgs e)
        {
            window.Show();
            window.WindowState = WindowState.Normal;
        }

        //TODO [CR BT] : Extract method into another class eg. NotifyUI which should be created.
        public void AddToListButton(string logMessage)
        {
            errors.AddItem(logMessage);
            if (window is ConfigurationWindow)
            {
                (window as ConfigurationWindow).errorList.ItemsSource = errors;
            }
        }
    }
}
