using ConnectPlay.TicketPlay.UI.App.Components.Base;

namespace ConnectPlay.TicketPlay.UI.App.Components;

public partial class WebsiteHeader : TranslatableComponent
{
    // We use protected so the WebsiteHeader.razor component can access this property
    protected string Language => T.CurrentLanguage.ToUpper();

    protected void ToggleLanguage()
    {
        if (T.CurrentLanguage == "nl")
        {
            T.SetLanguage("en");
        }
        else
        {
            T.SetLanguage("nl");
        }
    }
}