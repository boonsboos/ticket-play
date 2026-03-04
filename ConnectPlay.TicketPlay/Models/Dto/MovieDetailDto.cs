namespace ConnectPlay.TicketPlay.Models.Dto;

public record MovieDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Genre { get; set; } = default!;
    public string PosterUrl { get; set; } = default!;
    public int Duration { get; set; }
    public int MinimumAge { get; set; }
}
