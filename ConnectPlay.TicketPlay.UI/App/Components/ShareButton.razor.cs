using ConnectPlay.TicketPlay.UI.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConnectPlay.TicketPlay.UI.App.Components;

public partial class ShareButton(IOptions<ApiConfiguration> options)
{
    [Parameter]
    public required RenderFragment ChildContent { get; set; }
    [Parameter, EditorRequired]
    public required string Text { get; set; }
    [Parameter, EditorRequired]
    public required string Platform { get; set; }

    private string GetShareUrl()
    {
        var encodedUrl = Uri.EscapeDataString(options.Value.BaseUrl);
        var encodedText = Uri.EscapeDataString(Text);
        return Platform.ToLower() switch
        {
            "twitter" => $"https://twitter.com/intent/tweet?text={encodedText}&url={encodedUrl}",
            "whatsapp" => $"https://wa.me/?text={encodedText}%20{encodedUrl}",
            "telegram" => $"https://t.me/share/url?url={encodedUrl}&text={encodedText}",
            "reddit" => $"https://reddit.com/submit?url={encodedUrl}&title={encodedText.TrimEnd('\n')}",
            _ => throw new InvalidOperationException("Unsupported platform: " + Platform)
        };
    }
}