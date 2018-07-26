

namespace Common.Helpers
{
    using Common.Constants;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Controls;

    //TODO [CR BT] Format code
    //TODO [CR BT] Remove unused usings
    //TODO [CR BT] Remove empty lines

    public class NotifyUI
    {
        private  Window _window;
        private ObservableHashSet<string> _configurationErrors = new ObservableHashSet<string>();
        private System.Windows.Controls.ListBox _displayConfigurationErrorsList;
        private static NotifyIcon _notifyIcon = new NotifyIcon();

       public NotifyUI()
        {

        }

       public NotifyUI(Window window, System.Windows.Controls.ListBox listbox)
        {
            this._window = window;
            _displayConfigurationErrorsList = listbox;
        }
        //TODO [CR BT] Please make it private
        //TODO [CR BT] Remove duplicate code from the next two methods;
        //TODO [CR BT] Give appropiate naming for the next two methods

        public void NotifyError(string notificationTitle, string notificationMessage)
        {
            _notifyIcon.BalloonTipTitle = notificationTitle;
            _notifyIcon.BalloonTipText = notificationMessage;
            //TODO [CR BT] Remove path
            var path = Directory.GetCurrentDirectory();
            var pathCombine = Path.Combine(PathConfiguration.GetResourcesFolder(ConfigurationMessages.ResourceFolderErrorIcon));
            _notifyIcon.Icon = new Icon(pathCombine);
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(5000);
            _notifyIcon.Click += NotifyIconClick;
            Fixes.SetNotifyIconText(_notifyIcon, notificationMessage);            
        }

        public void BasicNotifyError(string notificationTitle, string notificationMessage)
        {
            _notifyIcon.BalloonTipTitle = notificationTitle;
            _notifyIcon.BalloonTipText = notificationMessage;
            var path = Directory.GetCurrentDirectory();
            var pathCombine = Path.Combine(PathConfiguration.GetResourcesFolder(ConfigurationMessages.ResourceFolderErrorIcon));
            _notifyIcon.Icon = new Icon(pathCombine);
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(5000);
            Fixes.SetNotifyIconText(_notifyIcon, notificationMessage);          
        }

        public void CatchErrorNotifier(Exception exception,string notifyMessage)
        {
            NotifyError(ConfigurationMessages.BadConfigurationTitle, notifyMessage);
            AddToListButton(notifyMessage);
            MyLogger.Logger.Debug(exception);
        }

        //TODO [CR BT] Please remove unused method
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

        //TODO [CR BT] Please make the method private
        public void AddToListButton(string logMessage)
        {
            _configurationErrors.AddItem(logMessage);
            _displayConfigurationErrorsList.ItemsSource = _configurationErrors;          
        }

        private void NotifyIconClick(object sender, EventArgs e)
        {
            _window.Show();
            _window.WindowState = WindowState.Normal;
        }

    }
}
