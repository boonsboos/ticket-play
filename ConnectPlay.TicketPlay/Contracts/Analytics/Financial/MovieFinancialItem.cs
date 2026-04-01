namespace ConnectPlay.TicketPlay.Contracts.Analytics.Financial;

public sealed record MovieFinancialItem
{
    public int MovieId { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public int Orders { get; init; }
    public int TicketsSold { get; init; }
    public decimal Revenue { get; init; }
}
