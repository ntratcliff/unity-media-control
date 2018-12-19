using UnityEditor;

namespace UnityMediaControl
{
    internal static partial class Preferences
    {
        private static class Prefs
        {
            /// <summary>
            /// Global enabled flag
            /// </summary>
            public static EditorPref<bool> Enabled = new EditorPref<bool>("enabled", true);

            /// <summary>
            /// Whether or not to resume media playback when play mode is paused
            /// </summary>
            public static EditorPref<bool> ResumeOnPause = new EditorPref<bool>("resumeOnPause", false);

            /// <summary>
            /// Whether or not to attempt to control Spotify directly
            /// </summary>
            public static EditorPref<bool> ControlSpotifyDirectly = new EditorPref<bool>("controlSpotifyDirectly", true);
        }

        /// <summary>
        /// Whether or not preferences are loaded
        /// </summary>
        public static bool Loaded { get; private set; }

        /// <summary>
        /// Is UMC enabled?
        /// </summary>
        public static bool Enabled { get { return Prefs.Enabled.Value; } }

        /// <summary>
        /// Whether or not to resume media playback when the editor is paused
        /// </summary>
        public static bool ResumeOnPause { get { return Prefs.ResumeOnPause.Value; } }

        /// <summary>
        /// Whether or not to attempt to control Spotify directly
        /// </summary>
        public static bool ControlSpotifyDirectly { get { return Prefs.ControlSpotifyDirectly.Value; } }

        static Preferences()
        {
            LoadPrefs();
        }

        /// <summary>
        /// Load all preferences from EditorPrefs
        /// </summary>
        private static void LoadPrefs()
        {
            LoadPref(Prefs.Enabled);
            LoadPref(Prefs.ResumeOnPause);
            LoadPref(Prefs.ControlSpotifyDirectly);

            Loaded = true;
        }

        /// <summary>
        /// Load a bool pref from EditorPrefs
        /// </summary>
        private static void LoadPref(EditorPref<bool> pref)
        {
            pref.Value = EditorPrefs.GetBool(pref.Key, pref.DefaultValue);
        }

        /// <summary>
        /// Save a bool pref to EditorPrefs
        /// </summary>
        private static void SavePref(EditorPref<bool> pref)
        {
            EditorPrefs.SetBool(pref.Key, pref.Value);
        }


        /// <summary>
        /// Holds information and data for an editor preference
        /// </summary>
        private class EditorPref<T>
        {
            public const string PREFIX = "UnityMediaControl_";
            public string Key { get; private set; }
            public T DefaultValue { get; private set; }
            public T Value;

            public EditorPref(string key, T defaultValue = default(T))
            {
                Key = PREFIX + key;
                DefaultValue = defaultValue;
                Value = defaultValue;
            }
        }
    }
}
