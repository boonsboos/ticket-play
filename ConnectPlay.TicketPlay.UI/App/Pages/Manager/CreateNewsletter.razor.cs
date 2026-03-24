using ConnectPlay.TicketPlay.Contracts.Newsletter;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Pages.Manager;

public partial class CreateNewsletter(INewsletterApi newsletterApi) : TranslatableComponent
{
    private readonly CreateNewsletterFormModel form = new();

    private string toastMessage = "";
    private string toastColor = "bg-success";
    private bool showToast = false;

    private bool isSubmitting = false;
    private bool showContentValidation = false;

    private async Task HandleSubmit()
    {
        showContentValidation = string.IsNullOrWhiteSpace(form.Content);

        if (showContentValidation)
        {
            return;
        }

        isSubmitting = true;
        try
        {
            var request = new CreateNewsletterRequest
            {
                Topic = form.Topic,
                Content = form.Content
            };

            await newsletterApi.SendNewsletterAsync(request);

            var subscribers = await newsletterApi.GetNewsletterSubscribersAsync();

            ShowSuccess($"Newsletter {request.Topic} was created. And send to {subscribers.Count()} subscribers.");
        }
        catch (ApiException e)
        {
            ShowError(e.Message);
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void ShowSuccess(string message)
    {
        toastMessage = message;
        toastColor = "bg-success";
        showToast = true;
    }

    private void ShowError(string message)
    {
        toastMessage = message;
        toastColor = "bg-danger";
        showToast = true;
    }

    public sealed class CreateNewsletterFormModel
    {
        public string Topic { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

    }
}