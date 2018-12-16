/* Noah Ratcliff - 2018 */

using UnityEditor;
using WinAppcommand = MediaControl.User32Interop.Appcommand;

namespace MediaControl
{
    [InitializeOnLoad]
    internal static class MediaControl
    {
        static MediaControl()
        {
            EditorApplication.playModeStateChanged += (state) =>
            {
                switch (state)
                {
                    case PlayModeStateChange.EnteredPlayMode:
                        // pause media
                        User32Interop.BroadcastAppcommand(WinAppcommand.MediaPause);
                        break;
                    case PlayModeStateChange.ExitingPlayMode:
                        // play media
                        User32Interop.BroadcastAppcommand(WinAppcommand.MediaPlay);
                        break;
                }
            };
        }
    }
}