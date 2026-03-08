namespace ConnectPlay.TicketPlay.Contracts.Hall;

public sealed record CreateHallResponse
{
    public int HallNumber { get; init; }
    public int Capacity { get; init; }
}