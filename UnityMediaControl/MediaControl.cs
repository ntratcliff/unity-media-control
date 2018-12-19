/* Noah Ratcliff - 2018 */

using UnityEditor;
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
            // add event listeners
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.pauseStateChanged += OnPauseStateChanged;
        }

        /// <summary>
        /// Called when the editor play state changes
        /// </summary>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    AttemptSetMediaPlaying(false); // pause
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    AttemptSetMediaPlaying(true); // play
                    break;
            }
        }

        /// <summary>
        /// Called when the editor pause state changes
        /// </summary>
        private static void OnPauseStateChanged(PauseState state)
        {
            // don't control media on pause if ResumeOnPause is not enabled
            if (!Preferences.ResumeOnPause) return;

            switch (state)
            {
                case PauseState.Paused:
                    AttemptSetMediaPlaying(true); // play
                    break;
                case PauseState.Unpaused:
                    AttemptSetMediaPlaying(false); // pause
                    break;
            }
        }

        /// <summary>
        /// Attempt to set the media play state
        /// </summary>
        /// <param name="playing">Whether or not media is playing</param>
        private static void AttemptSetMediaPlaying(bool playing)
        {
            // don't do anything if not enabled
            if (!Preferences.Enabled) return;

            // don't unpause if we didn't pause
            if (!pausedMedia && playing) return;

            // do some checks to see if we need to change play state
            if (!pausedMedia || !playing) // if we didn't already pause or are trying to pause
            {
                // is editor muted?
                if (EditorUtility.audioMasterMute) return;
            }

            if (Preferences.CheckForSpotify)
            {
                // check if spotify is already in the desired state (prevents accidental unpause)
                Spotify.Instance.Refresh();

                if (Spotify.Instance.IsPlaying == playing) return;
            }

            // set play state
            if (playing)
            {
                User32Interop.BroadcastAppcommand(WinAppcommand.MediaPlay);
            }
            else
            {
                User32Interop.BroadcastAppcommand(WinAppcommand.MediaPause);
            }

            pausedMedia = !playing;
        }
    }
}