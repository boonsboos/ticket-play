using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectPlay.TicketPlay.Models.Dto;

public record CreateScreeningDto
{
    public required int MovieId { get; init; }
    public required int HallId { get; init; }
    public required DateTimeOffset Time { get; init; }
}