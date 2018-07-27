namespace Common.Helpers
{
    using System;
    using System.Windows.Forms;
    using System.Reflection;
    using Common.Constants;

    public static class NotifyIconTextCharactersExpend
    {
        public static void SetNotifyIconText(NotifyIcon notification, string text)
        {
            if (text.Length >= 256) throw new ArgumentOutOfRangeException(NotifyIconTextMessages.TextLimited);
            if (text == null) throw new ArgumentNullException(NotifyIconTextMessages.NullObject);
            Type notifyIconType = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            notifyIconType.GetField(NotifyIconTextMessages.Text, hidden).SetValue(notification, text);
            if ((bool)notifyIconType.GetField(NotifyIconTextMessages.Added, hidden).GetValue(notification))
                notifyIconType.GetMethod(NotifyIconTextMessages.UdateImg, hidden).Invoke(notification, new object[] { true });
        }
    }
}
