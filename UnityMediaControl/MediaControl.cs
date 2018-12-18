/* Noah Ratcliff - 2018 */

using System;
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;
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
            if (!pausedMedia || !playing) // if we didn't already pause or are trying to pause
            {
                // is editor muted?
                if (EditorUtility.audioMasterMute) return;
            }

            // spotify is running, check if it is already in the desired state (prevents accidental unpause)
            if (Preferences.CheckForSpotify && IsSpotifyRunning() && IsSpotifyPlaying() == playing) return;

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

        /// <summary>
        /// Get the Spotify process
        /// </summary>
        private static string GetSpotifyTitle()
        {
            IntPtr handle = User32Interop.GetWindowByProcessName("Spotify");
            if (handle != IntPtr.Zero)
                return User32Interop.GetWindowText(handle);
            return null;
        }

        /// <summary>
        /// Checks if Spotify is running
        /// </summary>
        private static bool IsSpotifyRunning()
        {
            return GetSpotifyTitle() != null;
        }

        private static bool IsSpotifyPlaying()
        {
            // check if Spotify title is song name or "Spotify"
            string title = GetSpotifyTitle();

            if (title == null) return false;

            //Debug.Log("Spotify main window title: " + title);
            return title != "Spotify";
        }
    }
}