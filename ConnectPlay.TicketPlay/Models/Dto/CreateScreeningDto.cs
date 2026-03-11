using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectPlay.TicketPlay.Models.Dto;

public record CreateScreeningDto
{
    public int MovieId { get; init; }
    public int HallId { get; init; }
    public DateTimeOffset Time { get; init; }
}