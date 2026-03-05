namespace ConnectPlay.TicketPlay.Contracts.Hall;

public sealed record CreateHallResponse
{
    public int HallNumber { get; set; }
    public int Capacity { get; set; }
}