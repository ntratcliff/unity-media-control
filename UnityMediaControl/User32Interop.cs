/* Noah Ratcliff - 2018 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace UnityMediaControl
{
    internal static class User32Interop
    {
        public enum Appcommand
        {
            MediaPlay = 46,
            MediaPause = 47,
        }

        public const int HWIND_BROADCAST = 0xffff;

        public const int WM_APPCOMMAND = 0x319;

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("User32.dll")]
        public static extern int FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern long GetClassName(IntPtr hWnd, StringBuilder lpClassName, long nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        public static int BroadcastAppcommand(Appcommand command)
        {
            return SendMessage(HWIND_BROADCAST, WM_APPCOMMAND, 0, (int)command << 16);
        }

        public static WindowDescription[] EnumerateWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            List<WindowDescription> windows = new List<WindowDescription>();

            EnumWindows((IntPtr hWind, int lParam) =>
            {
                if (hWind == shellWindow) return true;
                if (!IsWindowVisible(hWind)) return true;

                // get window text
                string text = GetWindowText(hWind);
                if (text == string.Empty) return true;

                // get window class name
                string className = GetWindowClassName(hWind);
                if (className == string.Empty) return true;

                // add to windows list
                windows.Add(new WindowDescription(hWind, text, className));
                return true;
            }, 0);

            return windows.ToArray();
        }

        private static string GetWindowText(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd);
            if (len == 0) return string.Empty;

            StringBuilder builder = new StringBuilder(len);
            GetWindowText(hWnd, builder, len + 1);

            return builder.ToString();
        }

        private static string GetWindowClassName(IntPtr hWnd)
        {
            const int maxLen = 1000;
            StringBuilder builder = new StringBuilder(maxLen + 5);
            GetClassName(hWnd, builder, maxLen + 2);

            return builder.ToString();
        }
    }

    internal struct WindowDescription
    {
        public IntPtr Handle { get; private set; }
        public string Name { get; private set; }
        public string ClassName { get; private set; }

        public WindowDescription(IntPtr hWind, string name = null, string className = null)
        {
            this.Handle = hWind;
            Name = name;
            ClassName = className;
        }
    }
}