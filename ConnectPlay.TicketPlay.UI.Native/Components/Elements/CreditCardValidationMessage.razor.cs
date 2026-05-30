using ConnectPlay.TicketPlay.UI.Native.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Elements;

// All of this code was slopped together by AI in TranslatedValidationMessage
// only edited to work in the MAUI context

public partial class CreditCardValidationMessage : ComponentBase, IDisposable
{
    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    [Parameter]
    public Expression<Func<object>>? For { get; set; }

    protected IEnumerable<string>? Messages { get; private set; }

    /// <summary>
    /// Indicates whether this component has already subscribed to the
    /// <see cref="EditContext.OnValidationStateChanged"/> event.
    /// Prevents duplicate subscriptions.
    /// </summary>
    private bool _isSubscribed;
    private bool disposedValue;

    /// <summary>
    /// Called by the Blazor framework whenever component parameters are set or updated.
    /// Ensures that validation messages are refreshed and that the component subscribes
    /// to validation state changes from the parent <see cref="EditContext"/>.
    /// </summary>
    protected override void OnParametersSet()
    {
        if (CurrentEditContext is null || For is null)
            return;

        UpdateMessages();

        if (!_isSubscribed)
        {
            CurrentEditContext.OnValidationStateChanged += ValidationStateChanged;
            _isSubscribed = true;
        }
    }

    /// <summary>
    /// Handles validation state changes raised by the parent <see cref="EditContext"/>.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="EditContext"/> that triggered the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="ValidationStateChangedEventArgs"/> containing event data.
    /// </param>
    private void ValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        UpdateMessages();
        InvokeAsync(StateHasChanged);
    }

    private void UpdateMessages()
    {
        if (CurrentEditContext is null || For is null)
            return;

        var fieldIdentifier = FieldIdentifier.Create(For);
        Messages = CurrentEditContext.GetValidationMessages(fieldIdentifier);
    }

    /// <summary>
    /// Translates website static translation keys from validation attributes into AppResources messages.
    /// Attributes requires a static string, so we cannot directly use AppResources keys in the attributes, but we can translate them here.
    /// </summary>
    /// <param name="oldTranslationKey">the old key</param>
    /// <returns>The localized string</returns>
    private string Translate(string oldTranslationKey)
        => oldTranslationKey switch
        {
            "payment.validation.nameRequired" => AppResources.Payment_ValidateNameRequired,
            "payment.validation.numberRequired" => AppResources.Payment_ValidateNumberRequired,
            "payment.validation.invalidNumber" => AppResources.Payment_ValidateNumberInvalid,
            "payment.validation.expiryRequired" => AppResources.Payment_ValidateExpiryRequired,
            "payment.validation.expiryPast" => AppResources.Payment_ValidateExpiryDue,
            "payment.validation.cvcRequired" => AppResources.Payment_ValidateCvcRequired,
            "payment.validation.cvcInvalid" => AppResources.Payment_ValidateCvcInvalid,
            _ => oldTranslationKey
        };

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_isSubscribed && CurrentEditContext is not null)
                {
                    CurrentEditContext.OnValidationStateChanged -= ValidationStateChanged;
                }
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}