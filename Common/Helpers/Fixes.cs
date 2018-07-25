
namespace Common.Helpers
{
    using System;
    using System.Windows.Forms;
    using System.Reflection;
    public class Fixes
    {
        public static void SetNotifyIconText(NotifyIcon notification, string text)
        {
            if (text.Length >= 256) throw new ArgumentOutOfRangeException("Text limited to 127 characters");
            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(notification, text);
            if ((bool)t.GetField("added", hidden).GetValue(notification))
                t.GetMethod("UpdateIcon", hidden).Invoke(notification, new object[] { true });
        }
    }
}
