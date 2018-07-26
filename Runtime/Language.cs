using System;
using System.Collections.Generic;

namespace HouraiTeahouse.Localization {

    /// <summary> A class of ScriptableObjects that simply stores a set of String-String key-value pairs corresponding to the
    /// localization keys and the respective localized strings for that one particular language. Specially created to be saved
    /// as an asset file that can be loaded dynamically via Resources. Cannot be created through the editor, must be generated
    /// with LocalizationGenerator. </summary>
    public sealed class Language  {

        Dictionary<string, string> _map;

        internal Language() { _map = new Dictionary<string, string>(); }

        /// <summary> 
        /// Creates an instance of Language from two sets of keys and values 
        /// </summary>
        /// <param name="values"> the key-value mapping of the Language </param>
        internal Language(IDictionary<string, string> values) {
            Update(values);
        } 

        /// <summary>
        /// Gets the name of the language.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary> 
        /// Gets an enumeration of all of the localization keys supported by the Language 
        /// </summary>
        public IEnumerable<string> Keys {
            get { return _map.Keys;}
        }

        /// <summary> 
        /// Gets a localized string for a specific localization key. 
        /// If the key does not exist, the raw key is returned.
        /// </summary>
        /// <param name="key"> the localization key to retrieve. </param>
        /// <returns> the localized string </returns>
        public string this[string key] {
            get {
                if (!_map.ContainsKey(key))
                    return key;
                string result = _map[key];
                return string.IsNullOrEmpty(result) ? key : result;
            }
        }

        /// <summary> 
        /// Updates the current Language from two sets of keys and values 
        /// </summary>
        /// <param name="values"> the values of the Language </param>
        internal void Update(IDictionary<string, string> values) {
            if (values == null)
                return;
            _map = new Dictionary<string, string>(values);
        }

        /// <summary> 
        /// Checks if the Langauge contains a specific localization key. 
        /// </summary>
        /// <param name="key"> the localizaiton key to check for </param>
        /// <returns> True if the Langauge can localize the key, false otherwise. </returns>
        public bool ContainsKey(string key) {
            return _map.ContainsKey(key);
        }

    }

}
