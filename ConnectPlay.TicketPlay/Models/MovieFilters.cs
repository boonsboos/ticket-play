using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectPlay.TicketPlay.Models;

public record MovieFilters
{
    public string? Genre { get; init; }
    public int? MinimumAge { get; init; }
}
