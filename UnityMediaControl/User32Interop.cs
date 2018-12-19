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

        /// <summary>
        /// Window handle to use when broadcasting to all windows
        /// </summary>
        public const int HWIND_BROADCAST = 0xffff;

        /// <summary>
        /// App command message constant for SendMessage
        /// </summary>
        public const int WM_APPCOMMAND = 0x319;

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="hWnd">Handle of the window to recieve the message</param>
        /// <param name="Msg">Message type</param>
        /// <param name="wParam">Message parameters</param>
        /// <param name="lParam">Message parameter, for <see cref="WM_APPCOMMAND"/> messages, pass the command here</param>
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// Gets the caption text (title) for a window
        /// </summary>
        /// <param name="hWnd">The window handle</param>
        /// <param name="lpString">A string builder to recieve the string</param>
        /// <param name="nMaxCount">The maximum capacity of the string builder</param>
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Returns the length of a window caption text (title)
        /// </summary>
        /// <param name="hWnd">The window handle</param>
        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        /// Finds the handle of a window matching the provided parameters
        /// </summary>
        /// <param name="lpClassName">The window's class name, won't be checked if null</param>
        /// <param name="lpWindowName">The window's name (title/text), won't be checked if null</param>
        /// <returns>The handle for the window</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Get the process id for a window
        /// </summary>
        /// <param name="hWnd">The window's handle</param>
        /// <param name="lpdwProcessId">The process handle holder</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);

        /// <summary>
        /// Enumerate all windows
        /// </summary>
        /// <param name="enumProc">The process handling enumeration</param>
        /// <param name="lParam">Parameters -- TODO: look this up</param>
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        /// <summary>
        /// Broadcast a WM_APPCOMMAND to all windows
        /// </summary>
        /// <param name="command">The command to broadcast</param>
        public static int BroadcastAppcommand(Appcommand command)
        {
            return SendMessage(HWIND_BROADCAST, WM_APPCOMMAND, 0, (int)command << 16);
        }

        /// <summary>
        /// Gets the window text caption (title/name)
        /// </summary>
        /// <param name="hWnd">The window's handle</param>
        public static string GetWindowText(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd);
            if (len == 0) return string.Empty;

            StringBuilder builder = new StringBuilder(len);
            GetWindowText(hWnd, builder, len + 1);

            return builder.ToString();
        }

        /// <summary>
        /// Gets a handle for a window by process name. 
        /// If more than one window with the same process are found,
        /// the first visible window found will be returned.
        /// </summary>
        /// <param name="name">The process name</param>
        /// <returns>The window handle</returns>
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
