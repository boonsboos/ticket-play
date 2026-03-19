using ConnectPlay.TicketPlay.Contracts.Movie;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Components.Overview;

public partial class OverviewMovieCard : ComponentBase
{
    [Parameter, EditorRequired] public required OverviewMovie CardMovie { get; set; }
}
