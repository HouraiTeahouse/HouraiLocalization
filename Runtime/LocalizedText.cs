using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HouraiTeahouse.Localization {

public class LocalizedText : MonoBehaviour {

  public enum CaseSetting {
    Leave, Uppercase, Lowercase
  }

  [SerializeField] TMP_Text _text;

  string _nativeText;

  static LanguageChangedEvent OnManagerLanguageChanged => LanguageManager.Instance.OnLanguageChanged;

  /// <summary> 
  /// The format for the localization string to be displayed in. 
  /// </summary>
  /// <see cref="string.Format" />
  [SerializeField] string _format;

  [SerializeField] CaseSetting _caseSetting = CaseSetting.Leave;

  string _translatedText;

  /// <summary> 
  /// The TMP Text object to display the localized string onto 
  /// </summary>
  public TMP_Text Text {
    get { return _text; }
    set { 
      _text = value; 
      SetText();
    }
  }

  /// <summary> 
  /// The localization key used to lookup the localized string. 
  /// </summary>
  protected string NativeText {
    get { return _nativeText; }
    set {
      _nativeText = value;
      if (value == null || Text == null) return;
      SetText();
    }
  }

  /// <summary> 
  /// Unity callback. Called once before the object's first frame. 
  /// </summary>
  void Awake() {
    if (Text == null) ResetComponents();
    enabled = Text != null;
    if (LanguageManager.Instance != null) {
      OnManagerLanguageChanged.AddListener(OnChangeLanguage);
    }
    NativeText = GetText();
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() {
    if (Text == null || Text.text == _translatedText) return;
    NativeText = GetText();
  }

  /// <summary> 
  /// Unity Callback. Called on the first frame before Update is called. 
  /// </summary>
  void Start() => SetText();

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  void OnDestroy() {
    if (LanguageManager.Instance != null) {
      OnManagerLanguageChanged.RemoveListener(OnChangeLanguage);
    }
  }

  /// <summary>
  /// Reset is called when the user hits the Reset button in the Inspector's
  /// context menu or when adding the component the first time.
  /// </summary>
  void Reset() => ResetComponents();

  void ResetComponents() {
    if (_text != null) return;
    _text = GetComponent<TMP_Text>();
  }

  void SetText(string text = null) {
    if (Text == null) return;
    if (string.IsNullOrEmpty(text)) {
      LanguageManager languageManager = LanguageManager.Instance;
      if (languageManager != null) {
        text = Process(languageManager.CurrentLanguage[NativeText]);
      } else {
        text = string.Empty;
      }
    }
    Text.text = Process(text);
    _translatedText = Text.text;
  }

  protected string GetText() => Text != null ? Text.text : string.Empty;

  /// <summary> 
  /// Events callback for when the system wide language is changed. 
  /// </summary>
  /// <param name="args"> the language set that was changed to. </param>
  void OnChangeLanguage(Language lang) {
    if (lang == null) return;
    SetText(lang[NativeText]);
  }

  string Process(string val) { 
    val = string.IsNullOrEmpty(_format) ? val : string.Format(_format, val); 
    switch(_caseSetting) {
      case CaseSetting.Uppercase: return val.ToUpper(); 
      case CaseSetting.Lowercase: return val.ToLower();
    }
    return val;
  }

}

}
