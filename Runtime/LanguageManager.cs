using HouraiTeahouse.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace HouraiTeahouse.Localization {

[Serializable]
public class LanguageChangedEvent : UnityEvent<Language> {}

/// <summary> 
/// Singleton MonoBehaviour that manages all of localization system. 
/// </summary>
public sealed class LanguageManager : MonoBehaviour {

  public const string FileExtension = ".json";

  string _storageDirectory;
  HashSet<string> _languages;

  public Option LanguageOption;

  public static LanguageManager Instance { get; private set; }

  [SerializeField]
  [Tooltip("The default language to use if the Player's current language is not supported")]
  string _defaultLanguage = "en";

  [SerializeField]
  [Tooltip("Destroy this object on scene changes?")]
  bool _dontDestroyOnLoad = false;

  [SerializeField]
  [Tooltip("The Resources directory to load the Language files from")]
  string localizationDirectory = "lang";

  [SerializeField] LanguageChangedEvent _onLanguageChanged;
  public LanguageChangedEvent OnLanguageChanged {
    get { return  _onLanguageChanged ?? (_onLanguageChanged = new LanguageChangedEvent()); }
  }

  /// <summary> 
  /// The currently used language. 
  /// </summary>
  public Language CurrentLanguage { get; private set; }

  /// <summary> 
  /// All available languages currently supported by the system. 
  /// </summary>
  public IEnumerable<string> AvailableLanguages => _languages ?? Enumerable.Empty<string>();

  /// <summary> 
  /// Gets an enumeration of all of the localizable keys. 
  /// </summary>
  public IEnumerable<string> Keys => CurrentLanguage.Keys;

  void SetLanguage(string langName, IDictionary<string, string> values) {
    var culture = CultureInfo.GetCultureInfo(langName);
    CurrentLanguage.Update(values);
    CurrentLanguage.CultureInfo = culture;
    OnLanguageChanged?.Invoke(CurrentLanguage);
    Debug.LogFormat($"Set language to {culture.DisplayName}");
  }

  string GetLanguagePath(string identifier) => Path.Combine(_storageDirectory, identifier + FileExtension);

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake() {
      Instance = this;

      CurrentLanguage = new Language();
#if HOURAI_EVENTS
      _eventManager = Mediator.Global;
#endif

      _storageDirectory = Path.Combine(Application.streamingAssetsPath, localizationDirectory);
      var languages = Directory.GetFiles(_storageDirectory);
      _languages = new HashSet<string>(from file in languages
                                      where file.EndsWith(FileExtension)
                                      select Path.GetFileNameWithoutExtension(file));

      SetupLanguageOption();

      string currentLang = GetStoredLangaugeId();
      if (!_languages.Contains(currentLang)) {
          Debug.LogFormat("No language data for \"{0}\" found. Loading default language: {1}", _defaultLanguage, currentLang);
          currentLang = _defaultLanguage;
      }
      LoadLanguage(currentLang);
      SetStoredLanguageId(currentLang);

      if (_dontDestroyOnLoad)
          DontDestroyOnLoad(this);
  }

  void SetupLanguageOption() {
    CultureInfo systemLang = Application.systemLanguage.ToCultureInfo();

    LanguageOption.Type = OptionType.Enum;
    LanguageOption.MinValue = int.MinValue;
    LanguageOption.MaxValue = int.MaxValue;
    LanguageOption.DefaultValue = systemLang.LCID;
    LanguageOption.EnumOptions = _languages.Select(lang => {
      var culture = CultureInfo.GetCultureInfo(lang);
      return new EnumOption { Value = culture.LCID, DisplayName = culture.DisplayName };
    }).OrderBy(opt => opt.DisplayName)
    .ToArray();

    LanguageOption.OnValueChanged.AddListener(() => {
      LoadLanguage(GetStoredLangaugeId());
    });
  }

  string GetStoredLangaugeId() {
    int lcid = LanguageOption.Get<int>();
    return CultureInfo.GetCultureInfo(lcid).Name;
  }

  void SetStoredLanguageId(string languageId) {
    var culture = CultureInfo.GetCultureInfo(languageId);
    LanguageOption.Set<int>(culture.LCID);
  }

  /// <summary> 
  /// Loads a new language given the Microsoft language identifier. 
  /// </summary>
  /// <param name="identifier"> the Microsoft identifier for a lanuguage </param>
  /// <returns> the localization language </returns>
  /// <exception cref="ArgumentNullException"> <paramref name="identifier" /> is null. </exception>
  /// <exception cref="InvalidOperationException"> the language specified by <paramref name="identifier" /> is not currently
  /// supported. </exception>
  public Language LoadLanguage(string identifier) {
      Argument.NotNull(identifier);
      identifier = identifier.ToLower();
      if (!_languages.Contains(identifier))
          throw new InvalidOperationException(string.Format("Language with identifier of {0} is not supported.", identifier));
      var languageValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(GetLanguagePath(identifier)));
      SetLanguage(identifier, languageValues);
      return CurrentLanguage;
  }

}

}
