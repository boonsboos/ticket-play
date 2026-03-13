namespace ConnectPlay.TicketPlay.Models.Dto;

public record ScreeningDto
{
    public DateTimeOffset Time { get; init; }
    public int Hall { get; init; }
    public int MovieId { get; init; }
}