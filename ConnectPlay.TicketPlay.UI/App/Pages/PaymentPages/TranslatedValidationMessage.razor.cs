// This file contains the logic for the TranslatedValidationMessage Blazor
// component. It is responsible for displaying the validation messages for a
// specific field inside an EditForm.
//
// - The 'For' parameter provides a lambda expression pointing to the bound
//   model property (e.g., () => Model.Name). This identifies which field the
//   component should show messages for.
// - The component receives the EditContext from the parent EditForm via a
//   CascadingParameter. This gives access to the form's validation state.
// - When validation occurs, Blazor triggers the EditContext's
//   OnValidationStateChanged event. This component listens to that event and
//   refreshes its displayed messages accordingly.
// - UpdateMessages() extracts only the validation messages that belong to the
//   targeted field and exposes them to the .razor markup.
// - The .razor view then handles translating those messages using the T[key]
//   helper provided by the TranslatableComponent base class.
// - Dispose() unsubscribes from EditContext events to prevent memory leaks when
//   the component is removed.

using ConnectPlay.TicketPlay.UI.App.Components.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace ConnectPlay.TicketPlay.UI.App.Pages.PaymentPages;

public partial class TranslatedValidationMessage : TranslatableComponent, IDisposable
{
    // Cascading EditContext from the parent EditForm.
    // This gives access to the validation state of the form.
    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    // Expression pointing to the specific model property to validate.
    // Example: () => _creditCardModel.Name
    [Parameter]
    public Expression<Func<object>>? For { get; set; }

    // Holds the current validation messages for this specific field
    protected IEnumerable<string>? Messages { get; private set; }

    // Flag to track if we've already subscribed to the EditContext's validation state changes
    private bool _isSubscribed;

    // Called when component parameters are set or updated
    // Ensures messages are updated for the current field
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

    // Event handler triggered when validation state changes
    // Updates the Messages collection and refreshes the UI
    private void ValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        UpdateMessages();
        InvokeAsync(StateHasChanged);
    }

    // Extracts validation messages for the specific field identified by 'For'
    // Uses the EditContext's GetValidationMessages method with the appropriate FieldIdentifier
    // Updates the Messages property with the current validation messages for that field
    private void UpdateMessages()
    {
        if (CurrentEditContext is null || For is null)
            return;

        var fieldIdentifier = FieldIdentifier.Create(For);
        Messages = CurrentEditContext.GetValidationMessages(fieldIdentifier);
    }

    // Unsubscribes from the EditContext's validation state changes to prevent memory leaks
    // This is important when the component is removed from the UI to avoid keeping references to it in the EditContext
    public new void Dispose()
    {
        if (_isSubscribed && CurrentEditContext is not null)
        {
            CurrentEditContext.OnValidationStateChanged -= ValidationStateChanged;
        }
    }
}