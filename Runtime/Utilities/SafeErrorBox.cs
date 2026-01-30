using System;
using System.Runtime.InteropServices;

namespace EasyGameFramework.Essentials
{
    public static class SafeErrorBox
    {
#if UNITY_STANDALONE_WIN
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        public static void Show(string message, string title = "Error")
        {
            MessageBox(IntPtr.Zero, message, title, 0);
        }
#elif UNITY_STANDALONE_OSX
        public static void Show(string message, string title = "Error")
        {
            System.Diagnostics.Process.Start("osascript", $"-e 'display dialog \"{message}\" with title \"{title}\" buttons {{\"OK\"}}'");
        }
#elif UNITY_STANDALONE_LINUX
        public static void Show(string message, string title = "Error")
        {
            System.Diagnostics.Process.Start("zenity", $"--error --text=\"{message}\" --title=\"{title}\"");
        }
#else
        public static void Show(string message, string title = "Error")
        {
            UnityEngine.Debug.LogError($"{title}: {message}");
        }
#endif
    }
}
