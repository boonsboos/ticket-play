using ConnectPlay.TicketPlay.UI.Native.Abstract;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages.PaymentPages;

public partial class Payment : ComponentBase
{
    private readonly IOrderFlowService _orderFlowService;
    private readonly NavigationManager _navigation;

    private readonly CreditCardModel _creditCardModel = new();

    private PaymentMethod _selectedMethod = PaymentMethod.Ideal;

    private string? _selectedBank;

    public Payment(IOrderFlowService orderFlowService, NavigationManager navigation)
    {
        _orderFlowService = orderFlowService;
        _navigation = navigation;
    }

    private void SetPaymentMethod(PaymentMethod method)
    {
        _selectedMethod = method;
        if (method != PaymentMethod.Ideal)
        {
            _selectedBank = null; // reset bank selection
        }
    }

    private async Task PayAsync()
    {
        await _orderFlowService.PayOrderAsync();
        _navigation.NavigateTo("/payment/success");
    }

    private bool IsCreditCardFormValid =>
        !string.IsNullOrWhiteSpace(_creditCardModel.Name) &&
        !string.IsNullOrWhiteSpace(_creditCardModel.Number) &&
        !string.IsNullOrWhiteSpace(_creditCardModel.Expiry) &&
        !string.IsNullOrWhiteSpace(_creditCardModel.CVC);

    private enum PaymentMethod
    {
        Ideal,
        CreditCard
    }

    /// <summary>
    /// Custom validation attribute to ensure that the provided date is either in the future or today.
    /// </summary>
    public class FutureOrTodayAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is null) return false;

            if (DateTime.TryParse(value.ToString(), out DateTime dateValue))
            {
                return dateValue.Date >= DateTimeOffset.UtcNow.Date;
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
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "payment.validation.numberRequired")]
        [CreditCard(ErrorMessage = "payment.validation.invalidNumber")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "payment.validation.expiryRequired")]
        [FutureOrToday(ErrorMessage = "payment.validation.expiryPast")]
        public string Expiry { get; set; } = string.Empty;

        [Required(ErrorMessage = "payment.validation.cvcRequired")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "payment.validation.cvcInvalid")]
        public string CVC { get; set; } = string.Empty;
    }
}