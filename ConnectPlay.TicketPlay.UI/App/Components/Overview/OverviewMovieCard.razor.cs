using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Components.Overview;

public partial class OverviewMovieCard : TranslatableComponent
{
    [Parameter, EditorRequired] public required OverviewMovie CardMovie { get; set; }
}