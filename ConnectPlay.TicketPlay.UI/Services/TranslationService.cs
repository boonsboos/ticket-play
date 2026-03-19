namespace ConnectPlay.TicketPlay.UI.Services;

// the primary constructor gets the configuration injected
// configuration is used to access the translation values from the appsettings.json file
public class TranslationService(IConfiguration configuration) 
{
    private string currentLanguage = "en";

    public event Action? OnLanguageChanged; // the event sends a signal to the UI to update the text with the new language
    public string CurrentLanguage => currentLanguage;

    // this method changes the current language when the user clicks on the language toggle-text
    /// <summary>
    /// Sets the current application language based on the specified language code.
    /// </summary>
    /// <remarks>Calling this method updates the application's language and triggers any registered language
    /// change notifications. This allows the user interface to refresh and display text in the selected
    /// language.</remarks>
    /// <param name="language">The language code to set as the current language. Supported values are "en" for English and "nl" for Dutch. If
    /// an unsupported value is provided, the language defaults to English.</param>
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

    /// <summary>
    /// Retrieves the localized translation for the specified translation key in the current language.
    /// </summary>
    /// <remarks>If a translation is missing for the specified key, the method returns the key itself to
    /// indicate the absence of a translation in the UI.</remarks>
    /// <param name="translationKey">The key identifying the text to translate, using dot notation (for example, "header.nav.movies"). Cannot be null
    /// or empty.</param>
    /// <returns>The translated text corresponding to the specified key if found; otherwise, returns the original translation
    /// key.</returns>
    public string GetTranslation(string translationKey) // the translationKey is used to find the translated text, "header.nav.movies"
    {
        var translatedText = configuration[$"Translations:{currentLanguage}:{translationKey.Replace(".", ":")}"];
        
        // we return translationKey so we can see in the UI that there is a missing translation 
        return string.IsNullOrWhiteSpace(translatedText) ? translationKey : translatedText;
    }

    /// <summary>
    /// Gets the localized translation string associated with the specified translation key.
    /// </summary>
    /// <remarks>
    /// This indexer provides a convenient way to access translations using array-like syntax. It is
    /// functionally equivalent to calling the <see cref="GetTranslation"/> method with the same key.
    /// </remarks>
    /// <param name="translationKey">
    /// The key that identifies the translation to retrieve. Cannot be null or empty.
    /// </param>
    /// <returns>
    /// The localized string corresponding to the specified translation key. Returns an empty string if the key is not found.
    /// </returns>
    public string this[string translationKey] => GetTranslation(translationKey);
}