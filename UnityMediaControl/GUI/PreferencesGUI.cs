using UnityEditor;
using UnityEngine;

namespace EditorMediaManager
{
    internal static partial class Preferences
    {
        [PreferenceItem(Constants.GUIName)]
        public static void PreferencesGUI()
        {
            if (!Loaded) LoadPrefs();

            PrefOptionGUI("Enabled", Prefs.Enabled, "Whether or not " + Constants.AssetName + " is enabled");

            EditorGUI.BeginDisabledGroup(!Enabled); // disable other options UMC is disabled

            PrefOptionGUI("Resume playback on editor pause", Prefs.ResumeOnPause, "Whether or not to unpause your media when you pause the editor");
            PrefOptionGUI("Control Spotify directly", Prefs.ControlSpotifyDirectly, "Attempts to control Spotify directly, instead of globally pausing/unpausing media. Enabling this should stop Spotify and other applications from accidentally unpausing. Disable this if you don't use Spotify.");

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
