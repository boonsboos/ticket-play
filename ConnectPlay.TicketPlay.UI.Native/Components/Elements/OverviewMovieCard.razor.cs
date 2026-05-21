using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.UI.Native.Extensions;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Elements;

public partial class OverviewMovieCard : ComponentBase
{
    [Parameter, EditorRequired] public required OverviewMovie CardMovie { get; set; }
}