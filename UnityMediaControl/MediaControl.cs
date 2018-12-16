/* Noah Ratcliff - 2018 */

using UnityEditor;
using UnityEngine;
using WinAppcommand = UnityMediaControl.User32Interop.Appcommand;

namespace UnityMediaControl
{
    [InitializeOnLoad]
    internal static class MediaControl
    {
        static MediaControl()
        {
            Debug.Log("Media Control Initialized!");
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