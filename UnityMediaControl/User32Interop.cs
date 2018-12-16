/* Noah Ratcliff - 2018 */

using System.Runtime.InteropServices;

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

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);

        public static int BroadcastAppcommand(Appcommand command)
        {
            return SendMessage(HWIND_BROADCAST, WM_APPCOMMAND, 0, (int)command << 16);
        }
    }
}