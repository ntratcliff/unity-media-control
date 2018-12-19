using UnityEditor;
using UnityEngine;

namespace UnityMediaControl
{
    internal static partial class Preferences
    {
        [PreferenceItem("Media Control")]
        public static void PreferencesGUI()
        {
            if (!Loaded) LoadPrefs();

            PrefOptionGUI("Enabled", Prefs.Enabled, "Whether or not Unity Media Control is enabled");

            EditorGUI.BeginDisabledGroup(!Enabled); // disable other options UMC is disabled

            PrefOptionGUI("Resume playback on editor pause", Prefs.ResumeOnPause, "Whether or not to unpause your media when you pause the editor");
            PrefOptionGUI("Check for Spotify", Prefs.CheckForSpotify, "Attempts to control Spotify directly, if it's running. Enabling should stop Spotify from accidentally unpausing if it wasn't playing when you entered playmode. Disable this if you don't use Spotify.");

            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Draw an option for a player pref
        /// </summary>
        private static void PrefOptionGUI(string label, EditorPref<bool> pref, string tooltip)
        {
            bool initial = pref.Value;
            pref.Value = EditorGUILayout.Toggle(new GUIContent(label, tooltip), pref.Value);

            // check if value changed
            if (initial != pref.Value)
            {
                SavePref(pref);
            }
        }

    }
}
