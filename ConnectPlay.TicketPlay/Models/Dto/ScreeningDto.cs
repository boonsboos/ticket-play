namespace ConnectPlay.TicketPlay.Models.Dto;

public record ScreeningsDto
{
    public DateTime Time { get; set; }
    public int Hall { get; set; }
}