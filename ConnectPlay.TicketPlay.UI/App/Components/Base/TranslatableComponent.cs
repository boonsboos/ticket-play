using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Components.Base;

public class TranslatableComponent : ComponentBase, IDisposable
{
    // with [Inject] we tell blazor get me the instance of TranslationService from the Dependency Injection container
    // a DI container is a place where all the objects are created and managed
    [Inject]
    protected TranslationService T { get; set; } = default!;

    // this method automatically starts when the component is created
    protected override void OnInitialized() // standard Blazor lifecycle method
    {
        T.OnLanguageChanged += UpdateUiOnLanguageChange; // listen for language changes
    }

    // virtual is used to allow classes override this method
    protected virtual void UpdateUiOnLanguageChange()
    {
        // with InvokeAsync we ensure the right Blazor UI-thread is executed
        // tells the UI to re render when the language is changed
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        T.OnLanguageChanged -= UpdateUiOnLanguageChange; // stops the listing for the language changes when the component is cleard
    }
}