using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectPlay.TicketPlay.Models.Dto.Ticket;

public record CreateTicketDto
{
    public TicketType TicketType { get; init; }
    public bool? IsForWheelchair { get; init; }
}
