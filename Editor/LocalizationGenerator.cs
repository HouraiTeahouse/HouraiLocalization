using Google.GData.Spreadsheets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Localization.Tables;

namespace HouraiTeahouse.Localization {

[CreateAssetMenu(menuName="Hourai Teahouse/Localization Generator")]
public class LocalizationGenerator : ScriptableObject {

    const string kSmartKey = "[smart]";

    [SerializeField]
    [Tooltip("The public Google Spreadsheets link to pull data from")]
    string GoogleLink;

    [SerializeField]
    [Tooltip("The column the key is stored on")]
    string _keyColumn;

    [SerializeField]
    [Tooltip("Columns in the spreadsheet to ignore")]
    string[] _ignoreColumns;

    [SerializeField]
    [Tooltip("The String Table Collection to save the results to")]
    string _stringTableCollection;

    [MenuItem("Hourai Teahouse/Localization/Generate All")]
    public static void GenerateAll() {
        foreach (var generator in EditorAssetUtil.LoadAll<LocalizationGenerator>()) {
            Assert.IsNotNull(generator);
            try {
                generator.Generate();
            } catch(Exception e) {
                Debug.LogError($"Failed to generate localization assets from {generator.name}");
                Debug.LogException(e);
            }
        }
    }

    static AssetTableCollection FindStringTableCollection(string name) {
        var allCollections = LocalizationEditorSettings.GetAssetTablesCollection<StringTable>();
        return allCollections.FirstOrDefault(col => col.TableName == name);
    }

    /// <summary> 
    /// Reads the Google Spreadsheet and generates/updates the StringSet asset files 
    /// </summary>
    public void Generate() {
        var collections = FindStringTableCollection(_stringTableCollection);
        Assert.IsNotNull(collections, $"No table for '{_stringTableCollection}' defined.");
        var stringTables = collections.Tables.OfType<StringTable>()
                                             .ToDictionary(t => t.LocaleIdentifier.Code.ToLower(), t => t);
        
        // Clear all entries from all tables
        collections.TableEntries.Clear();
        foreach (var table in stringTables.Values) {
            table.Clear();
        }

        ListFeed test = GDocService.GetSpreadsheet(GoogleLink);
        var ignore = new HashSet<string>(_ignoreColumns);
        foreach (ListEntry row in test.Entries) {
            var keyEntry =
                row.Elements.OfType<ListEntry.Custom>()
                    .FirstOrDefault(e => e.LocalName.ToLower() == _keyColumn.ToLower());
            if (keyEntry == null) continue;
            var smart = keyEntry.Value.ToLower().Contains(kSmartKey);
            var key = keyEntry.Value.Replace(kSmartKey, "").Trim();
            foreach (ListEntry.Custom element in row.Elements) {
                string lang = element.LocalName.ToLower();
                StringTable table;
                if (ignore.Contains(lang) || !stringTables.TryGetValue(lang, out table)) continue;
                table.AddEntry(key, element.Value);
            }
        }

        // Mark all of the tables as dirty.
        foreach (var table in stringTables.Values) {
            EditorUtility.SetDirty(table);
            Debug.Log($"[Localization] Updated string table {table.LocaleIdentifier.Code} for '{_stringTableCollection}'");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

}

}
