/* Noah Ratcliff - 2018 */

using System;
using System.Diagnostics;
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
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        public static int BroadcastAppcommand(Appcommand command)
        {
            return SendMessage(HWIND_BROADCAST, WM_APPCOMMAND, 0, (int)command << 16);
        }

        public static string GetWindowText(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd);
            if (len == 0) return string.Empty;

            StringBuilder builder = new StringBuilder(len);
            GetWindowText(hWnd, builder, len + 1);

            return builder.ToString();
        }

        public static IntPtr GetWindowByProcessName(string name)
        {
            IntPtr found = IntPtr.Zero;
            EnumWindows((IntPtr wnd, int param) =>
            {
                if (found != IntPtr.Zero) return true;

                if (string.IsNullOrEmpty(GetWindowText(wnd))) return true;

                int processId = 0;
                GetWindowThreadProcessId(wnd, out processId);

                if (processId == 0) return true;

                try
                {
                    Process process = Process.GetProcessById(processId);
                    if (process.ProcessName == name)
                    {
                        found = wnd;
                        return true;
                    }
                }
                catch(ArgumentException e)
                {
                    //UnityEngine.Debug.Log(e.Message);
                }


                return true;
            }, IntPtr.Zero);

            return found;
        }
    }
}
