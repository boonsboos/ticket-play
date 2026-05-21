using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages;

public partial class Tickets : ComponentBase
{
    private readonly IEnumerable<Ticket> _tickets = [];
}
