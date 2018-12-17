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
            EditorApplication.pauseStateChanged += OnPauseStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    AttemptSetMediaPlaying(false);
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    AttemptSetMediaPlaying(true);
                    break;
            }
        }

        private static void OnPauseStateChanged(PauseState state)
        {
            if (!Preferences.ResumeOnPause) return;

            switch (state)
            {
                case PauseState.Paused:
                    AttemptSetMediaPlaying(true);
                    break;
                case PauseState.Unpaused:
                    AttemptSetMediaPlaying(false);
                    break;
            }
        }

        private static void AttemptSetMediaPlaying(bool playing)
        {
            // don't do anything if not enabled
            if (!Preferences.Enabled) return;

            // don't unpause if we didn't pause
            if (!pausedMedia && playing) return;

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