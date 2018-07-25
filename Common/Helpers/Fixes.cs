
namespace Common.Helpers
{
    using System;
    using System.Windows.Forms;
    using System.Reflection;

    //TODO [CR BT] Please give specific naming for the class
    //TODO [CR BT] Check for null
    //TODO [CR BT] Extract constants
    //TODO [CR BT] Used meaningful naming for variables instead of t
    //TODO [CR BT] Format code, remove empty lines
    //TODO [CR BT] Make class static

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
