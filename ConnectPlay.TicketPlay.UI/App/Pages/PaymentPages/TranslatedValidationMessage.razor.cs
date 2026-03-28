using ConnectPlay.TicketPlay.UI.App.Components.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace ConnectPlay.TicketPlay.UI.App.Pages.PaymentPages;


/// <summary>
/// Blazor component that displays translated validation messages for a specific
/// model field inside an <see cref="EditForm"/>.
/// </summary>
public partial class TranslatedValidationMessage : TranslatableComponent, IDisposable
{

    /// <summary>
    /// Gets the <see cref="EditContext"/> cascaded from the parent <c>EditForm</c>.
    /// Provides access to the form’s validation and field-level messages.
    /// </summary>
    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    /// <summary>
    /// Expression that identifies the bound model property this component displays
    /// validation messages for.
    /// </summary>
    /// <remarks>
    /// Example usage in Razor:
    /// <code>
    /// <TranslatedValidationMessage For="() => model.Property" />
    /// </code>
    /// </remarks>
    [Parameter]
    public Expression<Func<object>>? For { get; set; }

    /// <summary>
    /// Contains the validation messages associated with the field referenced by
    /// the <see cref="For"/> expression.
    /// </summary>
    protected IEnumerable<string>? Messages { get; private set; }

    /// <summary>
    /// Indicates whether this component has already subscribed to the
    /// <see cref="EditContext.OnValidationStateChanged"/> event.
    /// Prevents duplicate subscriptions.
    /// </summary>
    private bool _isSubscribed;

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
    /// <remarks>
    /// This method is triggered whenever the form performs validation — for example
    /// when a field loses focus, when input changes, or when <c>EditForm</c> performs
    /// a validation cycle.
    /// <para>
    /// The method refreshes the validation messages for the bound field by calling
    /// <see cref="UpdateMessages"/> and then requests a UI re-render using
    /// <see cref="InvokeAsync(Func{Task})"/> with <see cref="StateHasChanged"/>.
    /// This ensures the translated validation messages are immediately reflected
    /// in the user interface.
    /// </para>
    /// </remarks>
    /// <param name="sender">
    /// The <see cref="EditContext"/> that triggered the event.
    /// </param>
    private void ValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        UpdateMessages();
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Updates the <see cref="Messages"/> collection with the current validation
    /// messages for the field specified by the <see cref="For"/> parameter.
    /// </summary
    private void UpdateMessages()
    {
        if (CurrentEditContext is null || For is null)
            return;

        var fieldIdentifier = FieldIdentifier.Create(For);
        Messages = CurrentEditContext.GetValidationMessages(fieldIdentifier);
    }

    /// <summary>
    /// Cleans up the component by unsubscribing from the EditContext's 
    /// <c>OnValidationStateChanged</c> event.
    /// </summary>
    /// <remarks>
    /// This prevents memory leaks and unintended event triggers when the component is
    /// removed from the UI. Without unsubscribing, the EditContext would continue to
    /// reference this component, keeping it alive in memory.
    /// </remarks>
    public new void Dispose()
    {
        if (_isSubscribed && CurrentEditContext is not null)
        {
            CurrentEditContext.OnValidationStateChanged -= ValidationStateChanged;
        }
    }
}