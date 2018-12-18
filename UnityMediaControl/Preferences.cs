﻿using System;
using UnityEditor;
using UnityEngine;

namespace UnityMediaControl
{
    internal static class Preferences
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
