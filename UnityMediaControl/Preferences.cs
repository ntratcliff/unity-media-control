using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityMediaControl
{
    internal static class Preferences
    {
        private static class Prefs
        {
            public const string PREFIX = "UnityMediaControl_";

            /// <summary>
            /// Global enabled flag
            /// </summary>
            public static EditorPref<bool> Enabled = new EditorPref<bool>("enabled", true);

            /// <summary>
            /// Whether or not to resume media playback when play mode is paused
            /// </summary>
            public static EditorPref<bool> ResumeOnPause = new EditorPref<bool>("resumeOnPause", false);

            /// <summary>
            /// How UMC targets media players to pause/play media
            /// </summary>
            public static EditorPref<int> TargetMode = new EditorPref<int>("targetMode", (int)WindowTargetMode.Broadcast);

            /// <summary>
            /// Target window classes
            /// </summary>
            public static EditorPrefList<string> TargetWindowClassNames = new EditorPrefList<string>("targetWindowClasses", string.Empty);

            /// <summary>
            /// Target window names
            /// </summary>
            public static EditorPrefList<string> TargetWindowNames = new EditorPrefList<string>("targetWindowNames", string.Empty);
        }

        public enum WindowTargetMode
        {
            /// <summary>
            /// Send pause and play commands to all windows
            /// </summary>
            Broadcast,

            /// <summary>
            /// Send pause and play commands to specific windows
            /// </summary>
            SpecificWindows,
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

        public static WindowTargetMode TargetMode { get { return (WindowTargetMode)Prefs.TargetMode.Value; } }

        static Preferences()
        {
            LoadPrefs();
        }

        [PreferenceItem("Media Control")]
        public static void PreferencesGUI()
        {
            if (!Loaded) LoadPrefs();

            PrefOptionGUI("Enabled", Prefs.Enabled, "Whether or not Unity Media Control is enabled");

            EditorGUI.BeginDisabledGroup(!Enabled); // disable other options UMC is disabled

            PrefOptionGUI("Resume playback on editor pause", Prefs.ResumeOnPause, "Whether or not to unpause your media when you pause the editor");

            // window target mode dropdown
            int initial = Prefs.TargetMode.Value;
            Prefs.TargetMode.Value = Convert.ToInt32(EditorGUILayout.EnumPopup(
                new GUIContent("Window Target Mode", "How UMC targets applications to play/pause media"), TargetMode));
            if(Prefs.TargetMode.Value != initial)
            {
                SavePref(Prefs.TargetMode);
            }

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
            if(initial != pref.Value)
            {
                SavePref(pref);
            }
        }

        private static void LoadPrefs()
        {
            LoadPref(Prefs.Enabled);
            LoadPref(Prefs.ResumeOnPause);
            LoadPref(Prefs.TargetMode);
            LoadPref(Prefs.TargetWindowNames);
            LoadPref(Prefs.TargetWindowClassNames);

            Loaded = true;
        }

        private static void LoadPref(EditorPref<bool> pref)
        {
            pref.Value = EditorPrefs.GetBool(pref.Key, pref.DefaultValue);
        }

        private static void LoadPref(EditorPref<int> pref)
        {
            pref.Value = EditorPrefs.GetInt(pref.Key, pref.DefaultValue);
        }

        private static void LoadPref(IPrefList prefList)
        {
            prefList.Load();
        }

        private static void SavePref(EditorPref<bool> pref)
        {
            EditorPrefs.SetBool(pref.Key, pref.Value);
        }

        private static void SavePref(EditorPref<int> pref)
        {
            EditorPrefs.SetInt(pref.Key, pref.Value);
        }

        private static void SavePref(IPrefList prefList)
        {
            prefList.Save();
        }
        

        /// <summary>
        /// Holds information and data for an editor preference
        /// </summary>
        private class EditorPref<T>
        {
            public string Key { get; private set; }
            public T DefaultValue { get; private set; }
            public T Value;

            public EditorPref(string key, T defaultValue = default(T))
            {
                Key = Prefs.PREFIX + key;
                DefaultValue = defaultValue;
                Value = defaultValue;
            }

            public void SetKeyExplicit(string key)
            {
                Key = key;
            }
        }

        private interface IPrefList
        {
            void Load();
            void Save();
        }

        private class EditorPrefList<T> : EditorPref<T>, IPrefList
        {
            public const char SEPARATOR = ',';

            public List<T> Values;

            public EditorPrefList(string key, T defaultValue = default(T))
                : base(key, defaultValue)
            {
                Values = new List<T>();
            }

            public void Load()
            {
                string valuesString = EditorPrefs.GetString(Key, string.Empty);

                string[] arr = valuesString.Split(SEPARATOR);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

                for (int i = 0; i < arr.Length; i++)
                {
                    Values.Add((T)converter.ConvertFromString(arr[i]));
                }
            }

            public void Save()
            {
                EditorPrefs.SetString(Key, GetValuesString());
            }

            /// <summary>
            /// Get the values list as a string for storing in EditorPrefs
            /// </summary>
            /// <returns></returns>
            private string GetValuesString()
            {
                string[] arr = new string[Values.Count];
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

                for (int i = 0; i < Values.Count; i++)
                {
                    arr[i] = converter.ConvertToString(Values[i]);
                }

                return string.Join(SEPARATOR.ToString(), arr);
            }
        }
    }
}
