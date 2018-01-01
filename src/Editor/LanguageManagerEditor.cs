using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Localization {

    /// <summary> A custom Editor for LanguageManager. </summary>
    [CustomEditor(typeof(LanguageManager))]
    internal class LanguageManagerEditor : Editor {

        string[] _availableLanguages;
        string[] _display;
        int _index;

        /// <summary>
        ///     <see cref="UnityEditor.Editor.OnInspectorGUI" />
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var langManager = target as LanguageManager;

            if (_availableLanguages == null) {
                _availableLanguages = langManager.AvailableLanguages.ToArray();
                _display = _availableLanguages.Select(Language.GetName).ToArray();
                Language language = langManager.CurrentLanguage;
                if (language != null)
                    _index = Array.LastIndexOf(_availableLanguages, langManager.CurrentLanguage.Name);
            }

            GUI.enabled = EditorApplication.isPlayingOrWillChangePlaymode;
            int oldIndex = _index;
            _index = EditorGUILayout.Popup("Current Language", _index, _display);
            if (_index != oldIndex)
                langManager.LoadLanguage(_availableLanguages[_index]);
            GUI.enabled = true;
        }

    }

}
