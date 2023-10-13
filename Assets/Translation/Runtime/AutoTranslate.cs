using TMPro;
using Translation.Runtime;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class AutoTranslate : MonoBehaviour, IOnLanguageChanged
{
    private TMP_Text _textElement;

    private string _originalString;

    public void OnLanguageChanged(MainTranslator.ELanguage newLanguage)
    {
        ProcessTranslation();
    }
    
    private void Awake()
    {
        if (!_textElement)
            _textElement = GetComponent<TMP_Text>();
        if (!_textElement)
        {
            Debug.LogError("No text found!");
            return;
        }
        _originalString = _textElement.text;
    }

    private void Start()
    {
        ProcessTranslation();
    }

    private void ProcessTranslation()
    {
        if (!_textElement)
            return;
        _textElement.text = _originalString.Translate();
    }
}