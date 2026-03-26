using ConnectPlay.TicketPlay.Contracts.Newsletter;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using System.ComponentModel.DataAnnotations;
using Refit;



namespace ConnectPlay.TicketPlay.UI.App.Pages.Manager;

public partial class CreateNewsletter(INewsletterApi newsletterApi) : TranslatableComponent
{
    private CreateNewsletterFormModel form = new();

    private string toastMessage = "";
    private string toastColor = "bg-success";
    private bool showToast = false;

    private bool isSubmitting = false;

    private async Task HandleSubmit()
    {
        isSubmitting = true;

        try
        {
            var request = new CreateNewsletterRequest
            {
                Topic = form.Topic!,
                Content = form.Content!
            };

            await newsletterApi.SendNewsletterAsync(request);

            var subscribersCount = await newsletterApi.GetNewsletterSubscriberCountAsync();

            ShowSuccess(string.Format(
                T["createNewsletter.successMessage"],
                request.Topic,
                subscribersCount
            ));

            form = new();
        }
        catch (ApiException e)
        {
            ShowError(e.Message);
        }
        catch (Exception e)
        {
            ShowError($"Unexpected error: {e.Message}");
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

    private void HideToast()
    {
        showToast = false;
    }

    public sealed class CreateNewsletterFormModel
    {
        [Required, StringLength(255)]
        public string? Topic { get; set; }
        [Required, StringLength(4000)]
        public string? Content { get; set; }

    }
}