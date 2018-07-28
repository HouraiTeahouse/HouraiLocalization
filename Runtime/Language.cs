using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace HouraiTeahouse.Localization {

/// <summary> 
/// A class of ScriptableObjects that simply stores a set of String-String key-value pairs corresponding to the
/// localization keys and the respective localized strings for that one particular language. Specially created to be saved
/// as an asset file that can be loaded dynamically via Resources. Cannot be created through the editor, must be generated
/// with LocalizationGenerator. 
/// </summary>
public sealed class Language  {

  Dictionary<string, string> _map;

  internal Language() { 
    _map = new Dictionary<string, string>(); 
  }

  /// <summary> 
  /// Creates an instance of Language from two sets of keys and values 
  /// </summary>
  /// <param name="values"> the key-value mapping of the Language </param>
  internal Language(IDictionary<string, string> values) {
    Update(values);
  } 

  /// <summary>
  /// Gets the CultureInfo of the target langauge.
  /// </summary>
  public CultureInfo CultureInfo { get; internal set; }

  /// <summary> 
  /// Gets an enumeration of all of the localization keys supported by the Language 
  /// </summary>
  public IEnumerable<string> Keys => _map.Keys;

  /// <summary> 
  /// Gets a localized string for a specific localization key. 
  /// If the key does not exist, the raw key is returned.
  /// </summary>
  /// <param name="key"> the localization key to retrieve. </param>
  /// <returns> the localized string </returns>
  public string this[string key] {
    get {
      string translation;
      if (!_map.TryGetValue(key, out translation)) {
        Debug.LogWarning($"Language '{CultureInfo.DisplayName}' does not have a translation for '{key}'");
        return key;
      }
      translation = string.IsNullOrEmpty(translation) ? key : translation;
      Debug.Log($"[Localization] Translated ({CultureInfo.DisplayName}) '{key}' -> '{translation}'");
      return translation;
    }
  }

  /// <summary> 
  /// Updates the current Language from two sets of keys and values 
  /// </summary>
  /// <param name="values"> the values of the Language </param>
  internal void Update(IDictionary<string, string> values) {
    if (values == null) return;
    _map = new Dictionary<string, string>(values);
  }

  /// <summary> 
  /// Checks if the Langauge contains a specific localization key. 
  /// </summary>
  /// <param name="key"> the localizaiton key to check for </param>
  /// <returns> True if the Langauge can localize the key, false otherwise. </returns>
  public bool ContainsKey(string key) => _map.ContainsKey(key);

}

}
