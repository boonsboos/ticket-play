using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Elements;

public partial class MovieCard : ComponentBase
{
    [Parameter, EditorRequired] public required Movie CardMovie { get; set; }
}
