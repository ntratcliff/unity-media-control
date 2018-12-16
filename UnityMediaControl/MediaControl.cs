/* Noah Ratcliff - 2018 */

using UnityEditor;
using UnityEngine;
using WinAppcommand = UnityMediaControl.User32Interop.Appcommand;

namespace UnityMediaControl
{
    [InitializeOnLoad]
    internal static class MediaControl
    {
        /// <summary>
        /// Whether or not we recently paused playing media
        /// </summary>
        private static bool pausedMedia;

        static MediaControl()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    OnEnterPlayMode();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    OnExitPlayMode();
                    break;
            }
        }

        private static void OnEnterPlayMode()
        {
            AttemptSetMediaPlaying(false);
        }

        private static void OnExitPlayMode()
        {
            AttemptSetMediaPlaying(true);
        }

        private static void AttemptSetMediaPlaying(bool playing)
        {

            // do some checks to see if we need to change play state
            if(!pausedMedia || !playing) // if we didn't already pause or are trying to pause
            {
                // is editor muted?
                if (EditorUtility.audioMasterMute) return;
            }

            if (playing)
            {
                // play media
                User32Interop.BroadcastAppcommand(WinAppcommand.MediaPlay);
            }
            else
            {
                // pause media
                User32Interop.BroadcastAppcommand(WinAppcommand.MediaPause);
            }

            pausedMedia = !playing;
        }
    }
}