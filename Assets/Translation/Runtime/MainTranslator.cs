using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Csv;
using Translation.Runtime;
using UnityEngine;

public class MainTranslator : MonoBehaviour
{
    public enum ELanguage
    {
        en_us = 0,
        en_uk = 1,
        pt_pt = 2,
        de_de = 3,
        fr_fr = 4,
        it_it = 5
    }

    public static MainTranslator Instance;

    public ELanguage currentLanguage;
    public ELanguage fallbackLanguage;

    public string translationResourceName = "translation";

    public Dictionary<string, Dictionary<ELanguage, string>> translationData;

    private const string IDRegExpression = @"\$([^$]+)\$";

    private readonly Regex _idRegex = new Regex(IDRegExpression);

    private ELanguage _currentLanguage;

    public string TranslateString(string text)
    {
        var matches = _idRegex.Matches(text);
        var translatedString = text;
        foreach (Match match in matches)
        {
            var stringId = match.Groups[1].Value;
            translatedString = translatedString.Replace($"${stringId}$",
                GetTranslatedString(stringId));
        }
        return translatedString;
    }

    private void Awake()
    {
        Instance = this;
        _currentLanguage = currentLanguage;
        UpdateTranslationData(translationResourceName);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (currentLanguage != _currentLanguage)
        {
            _currentLanguage = currentLanguage;
            var listeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IOnLanguageChanged>().ToList();
            listeners.ForEach(x => x.OnLanguageChanged(_currentLanguage));
        }
    }
#endif

    private string GetTranslatedString(string id)
    {
        if (!translationData.ContainsKey(id))
        {
            Debug.LogError($"No string id '{id}' found!");
            return $"${id}$";
        }
        return translationData[id][_currentLanguage];
    }

    private void UpdateTranslationData(string resourceName)
    {
        translationData = new Dictionary<string, Dictionary<ELanguage, string>>();
        var allText = Resources.Load<TextAsset>(resourceName);
        if (!allText)
        {
            Debug.LogError($"No translation resource found with name '{resourceName}', " +
                           "make sure to include one on the 'Resources' folder!");
            return;
        }
        var lines = CsvReader.ReadFromText(allText.text).ToList();
        if (lines.Count == 0)
        {
            Debug.LogError($"Translation file '{resourceName}' was empty!");
            return;
        }

        foreach (var csvLine in lines)
        {
            var headers = csvLine.Headers;
            var values = csvLine.Values;

            var translations = new Dictionary<ELanguage, string>();
            var id = values[0];
            for (var j = 1; j < values.Length; j++)
            {
                if (Enum.TryParse(headers[j], out ELanguage lan))
                {
                    translations.Add(lan, values[j]);
                }
                else
                {
                    Debug.LogError($"'{headers[j]}' is not a valid language!");
                }
            }
            translationData.Add(id, translations);
        }
    }
}