namespace ConnectPlay.TicketPlay.UI.Services;

// the primary constructor gets the configuration injected
// configuration is used to access the translation values from the appsettings.json file
public class TranslationService(IConfiguration configuration) 
{
    private string currentLanguage = "en";

    public event Action? OnLanguageChanged; // the event sends a signal to the UI to update the text with the new language
    public string CurrentLanguage => currentLanguage;

    // this method change the current language when the user cliks on the language toggle-text
    public void SetLanguage(string language)
    {
        if (language != "en" && language != "nl")
        {
            language = "en"; // fallback to default language
        }

        currentLanguage = language;

        // Invoke notifying the UI that the language is changed
        // it triggers the UI to update the text with the new language
        // ? means so only invoke the event if there are listeners
        OnLanguageChanged?.Invoke();
    }

    public string GetTranslation(string translationKey) // the translationKey is used to find the translated text, "header.nav.movies"
    {
        var translatedText = configuration[$"Translations:{currentLanguage}:{translationKey.Replace(".", ":")}"];
        
        // we return translationKey so we can see in the UI that there is a missing translation 
        return string.IsNullOrWhiteSpace(translatedText) ? translationKey : translatedText;
    }

    // this is called an indexer
    // we can use T["header.nav.movies"] instead of T.GetTranslation("header.nav.movies") in the UI
    public string this[string translationKey] => GetTranslation(translationKey);
}