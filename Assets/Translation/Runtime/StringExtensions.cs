using UnityEngine;
public static class StringExtensions
{
    public static string Translate(this string stringToTranslate)
    {
        if (MainTranslator.Instance) return MainTranslator.Instance.TranslateString(stringToTranslate);
        Debug.LogError("No 'MainTranslator' instance found! Make sure to add the object to the scene");
        return stringToTranslate;
    }
}