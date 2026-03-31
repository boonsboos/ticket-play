using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using Refit;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ConnectPlay.TicketPlay.UI.App.Components;

public partial class WebsiteFooter(
    INewsletterApi newsletterApi) : TranslatableComponent
{
    private readonly INewsletterApi _newsletterApi = newsletterApi;

    protected CreateSubscriberFormModel form = new();
    protected bool isSubmitting = false;

    protected string toastMessage = "";
    protected string toastColor = "bg-success";
    protected bool showToast = false;

    protected async Task HandleSubmit()
    {
        HideToast();

        if (!ValidateForm())
        {
            return;
        }

        isSubmitting = true;

        try
        {
            var subscriber = new NewsletterSubscriber
            {
                Email = form.Email,
                Name = form.Name,
            };

            await _newsletterApi.CreateSubscriberAsync(subscriber);

            ShowSuccess(T["createNewsletterSubscriber.successMessage"]);
            form = new CreateSubscriberFormModel();
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            ShowError(T["createNewsletterSubscriber.errorMessage"]);
        }
        catch (ApiException ex)
        {
            ShowError($"API error: {ex.Content}");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(form.Name))
        {
            ShowError(T["createNewsletterSubscriber.emptyNameField"]);
            return false;
        }

        if (string.IsNullOrWhiteSpace(form.Email))
        {
            ShowError(T["createNewsletterSubscriber.emptyEmailField"]);
            return false;
        }

        var emailValidator = new EmailAddressAttribute();

        if (!emailValidator.IsValid(form.Email))
        {
            ShowError(T["createNewsletterSubscriber.invalidEmail"]);
            return false;
        }

        return true;
    }

    protected void ShowSuccess(string message)
    {
        toastMessage = message;
        toastColor = "bg-success";
        showToast = true;
    }

    protected void ShowError(string message)
    {
        toastMessage = message;
        toastColor = "bg-danger";
        showToast = true;
    }

    protected void HideToast() => showToast = false;

    public sealed class CreateSubscriberFormModel
    {
        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
