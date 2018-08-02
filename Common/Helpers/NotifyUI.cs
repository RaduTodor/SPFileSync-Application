namespace Common.Helpers
{
    using Common.Constants;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;

    /// <summary>
    ///     An instance of NotifyUI class is used for notifying in tray bar when an error occurs or when an operation is happening.
    /// </summary>
    public class NotifyUI
    {
        private Window _window;
        private ObservableHashSet<string> _configurationErrors;
        private System.Windows.Controls.ListBox _displayConfigurationErrorsList;
        private static NotifyIcon _notifyIcon = new NotifyIcon();

        public NotifyUI()
        {
            _configurationErrors = new ObservableHashSet<string>();
        }

        public NotifyUI(Window window, System.Windows.Controls.ListBox listbox)
        {
            this._window = window;
            _displayConfigurationErrorsList = listbox;
            _configurationErrors = new ObservableHashSet<string>();
        }
        /// <summary>
        /// NotifyUserWithTrayBarBalloon method is used to notify a message in tray bar.     
        /// </summary>
        public void NotifyUserWithTrayBarBalloon(string notificationTitle, string notificationMessage)
        {
            _notifyIcon.BalloonTipTitle = notificationTitle;
            _notifyIcon.BalloonTipText = notificationMessage;
            var pathCombine = Path.Combine(PathConfiguration.GetApplicationDirectory(ConfigurationMessages.ResourceFolderErrorIcon));
            _notifyIcon.Icon = new Icon(pathCombine);
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(5000);
            _notifyIcon.Click += NotifyIconClick;
            NotifyIconTextCharactersExpend.SetNotifyIconText(_notifyIcon, notificationMessage);
        }
        /// <summary>
        /// CatchErrorNotifier method is used to notify an error in tray bar.     
        /// </summary>
        public void CatchErrorNotifier(Exception exception, string notifyMessage)
        {
            NotifyUserWithTrayBarBalloon(ConfigurationMessages.BadConfigurationTitle, notifyMessage);
            AddToListButton(notifyMessage);
            LoggerManager.Logger.Debug(exception);
        }

        private void AddToListButton(string logMessage)
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
