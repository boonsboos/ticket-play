using ConnectPlay.TicketPlay.UI.App.Components.Base;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.UI.App.Pages.PaymentPages;

/// <summary>
/// Represents the payment page of the application, allowing the user to choose
/// and complete a payment method (iDEAL or credit card) for the current order.
/// </summary>
public partial class Payment : TranslatableComponent
{
    private readonly WebsiteService _websiteService;
    private readonly NavigationManager _navigation;

    /// <summary>
    /// Initializes a new instance of the <see cref="Payment"/> component and injects
    /// the required application services.
    /// </summary>
    /// <param name="websiteService">
    /// The <see cref="WebsiteService"/> used to process and mark the current order
    /// as paid when a payment method is submitted.
    /// </param>
    /// <param name="navigation">
    /// The <see cref="NavigationManager"/> used to navigate the user to the success
    /// page after completing the payment.
    /// </param>
    public Payment(WebsiteService websiteService, NavigationManager navigation)
    {
        _websiteService = websiteService;
        _navigation = navigation;
    }

    private string _selectedMethod = "iDEAL";

    private string? _selectedBank;

    private CreditCardModel _creditCardModel = new();

    private void OnPaymentMethodChanged(ChangeEventArgs e)
    {
        _selectedMethod = e.Value?.ToString() ?? "iDEAL";
        _selectedBank = null; // reset bank selection
    }

    private async Task PayWithCreditCard()
    {
        await _websiteService.PayOrder();
        _navigation.NavigateTo("/payment/success");
    }

    private async Task PayWithiDEAL()
    {
        await _websiteService.PayOrder();
        _navigation.NavigateTo("/payment/success");
    }

    /// Checks if the credit card form is fully filled
    /// Used to enable/disable the submit button
    private bool IsCreditCardFormValid =>
        !string.IsNullOrWhiteSpace(_creditCardModel.Name) &&
        !string.IsNullOrWhiteSpace(_creditCardModel.Number) &&
        !string.IsNullOrWhiteSpace(_creditCardModel.Expiry) &&
        !string.IsNullOrWhiteSpace(_creditCardModel.CVC);

    /// <summary>
    /// Validation attribute that checks whether a date is today or lies in the future.
    /// </summary>
    /// <remarks>
    /// This attribute does not validate null or empty values; that responsibility belongs
    /// to attributes such as <see cref="RequiredAttribute"/>.  
    /// <para>
    /// The attribute attempts to parse the provided value into a <see cref="DateTime"/>.
    /// If parsing succeeds, the date is considered valid only when it represents today
    /// or a future date. Past dates are marked as invalid.
    /// </para>
    /// </remarks>
    public class FutureOrTodayAttribute : ValidationAttribute
    {
        /// <summary>
        /// Determines whether the provided value is valid. A value is considered valid if:
        /// <list type="bullet">
        ///     <item><description>It is <c>null</c> (handled by other validation attributes).</description></item>
        ///     <item><description>It can be parsed into a <see cref="DateTime"/>.</description></item>
        ///     <item><description>The parsed date represents today or a future date.</description></item>
        /// </list>
        /// </summary>
        /// <param name="value">The value being validated, typically a string representation of a date.</param>
        /// <returns>
        /// <c>true</c> if the date is today or in the future; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid(object? value)
        {
            // If the value is null, consider it valid.
            // [Required] should handle empty/null validation separately.
            if (value == null) return true;

            // Attempt to parse the value to a DateTime object.
            if (DateTime.TryParse(value.ToString(), out DateTime dateValue))
            {
                // Returns true if the date is today or later, false if it's in the past.
                return dateValue.Date >= DateTime.Today;
            }
            return false;
        }
    }

    /// <summary>
    /// Represents the input fields required for credit card payment.
    /// This model is bound to the payment form and uses DataAnnotations
    /// </summary>
    public class CreditCardModel
    {
        [Required(ErrorMessage = "payment.validation.nameRequired")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "payment.validation.numberRequired")]
        [CreditCard(ErrorMessage = "payment.validation.invalidNumber")]
        [Display(Name = "Number")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "payment.validation.expiryRequired")]
        [FutureOrToday(ErrorMessage = "payment.validation.expiryPast")]
        [Display(Name = "Expiry")]
        public string Expiry { get; set; } = string.Empty;

        [Required(ErrorMessage = "payment.validation.cvcRequired")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "payment.validation.cvcInvalid")]
        [Display(Name = "CVC")]
        public string CVC { get; set; } = string.Empty;
    }
}