namespace Translation.Runtime
{
    public interface IOnLanguageChanged
    {
        public void OnLanguageChanged(MainTranslator.ELanguage newLanguage);
    }
}