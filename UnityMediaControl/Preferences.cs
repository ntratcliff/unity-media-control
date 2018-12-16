using System;
using UnityEditor;

namespace UnityMediaControl
{
    internal static class Preferences
    {
        private static class Prefs
        {
            public static EditorPref<bool> Enabled = new EditorPref<bool>("enabled", true);
        }

        /// <summary>
        /// Whether or not preferences are loaded
        /// </summary>
        public static bool Loaded { get; private set; }

        /// <summary>
        /// Is UMC enabled?
        /// </summary>
        public static bool Enabled { get { return Prefs.Enabled.Value; } }

        static Preferences()
        {
            LoadPrefs();
        }

        [PreferenceItem("Unity Media Control")]
        public static void PreferencesGUI()
        {
            if (!Loaded) LoadPrefs();

            PrefOptionGUI("Enabled", Prefs.Enabled);
        }

        /// <summary>
        /// Draw an option for a player pref
        /// </summary>
        private static void PrefOptionGUI(string label, EditorPref<bool> pref)
        {
            bool initial = pref.Value;
            pref.Value = EditorGUILayout.Toggle(label, pref.Value);
            
            // check if value changed
            if(initial != pref.Value)
            {
                SavePref(pref);
            }
        }

        private static void LoadPrefs()
        {
            LoadPref(Prefs.Enabled);

            Loaded = true;
        }

        private static void LoadPref(EditorPref<bool> pref)
        {
            pref.Value = EditorPrefs.GetBool(pref.Key, pref.DefaultValue);
        }

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
