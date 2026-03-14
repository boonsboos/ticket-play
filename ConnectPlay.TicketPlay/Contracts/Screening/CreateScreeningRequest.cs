namespace ConnectPlay.TicketPlay.Contracts.Screening;

public record CreateScreeningRequest
{
    public required int MovieId { get; init; }
    public required int HallId { get; init; }
    public required DateTimeOffset Time { get; init; }
    public bool SneakPreview { get; init; }
}