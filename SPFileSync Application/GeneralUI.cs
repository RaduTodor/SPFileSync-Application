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
    public class GeneralUI
    {
        private Window window;
        private Utils.ObservableHashSet<string> errors = new Utils.ObservableHashSet<string>();
        public GeneralUI(Window window)
        {
            this.window = window;
        }

        public bool FieldValidation(string data, System.Windows.Controls.Label displayError)
        {
            bool verifyData = true;
            if (string.IsNullOrEmpty(data))
            {
                verifyData = false;
                DisplayWarning(displayError, "Empty field");
            }
            return verifyData;
        }

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

        public void NotifyError(NotifyIcon notifyIcon, string notificationTitle, string notificationMessage)
        {
            notifyIcon.BalloonTipTitle = notificationTitle;
            notifyIcon.BalloonTipText = notificationMessage;
            var path = Directory.GetCurrentDirectory();
            var pathCombine = Path.Combine(path, "error.ico");
            notifyIcon.Icon = new Icon(pathCombine);
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(5000);
            notifyIcon.Click += NotifyIconClick;
            notifyIcon.Text = notificationMessage;
        }

        private void NotifyIconClick(object sender, EventArgs e)
        {
            window.Show();
            window.WindowState = WindowState.Normal;
        }

        public void AddToListButton(string logMessage)
        {
            errors.AddItem(logMessage);
            (window as ConfigurationWindow).errorList.ItemsSource = errors;
        }
    }
}
