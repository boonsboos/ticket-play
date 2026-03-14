using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace ConnectPlay.TicketPlay.UI.App.Components;

public partial class WebsiteHeader : ComponentBase // ComponentBase is the baseclass for a Blazor component
{
    // We use protected so the WebsiteHeader.razor component can access this property
    protected string Language { get; set; } = "NL";

    protected void ToggleLanguage()
    {
        if (Language == "NL")
        {
            Language = "ENG";
        }
        else
        {
            Language = "NL";
        }
    }
}