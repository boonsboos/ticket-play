using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("movies")]
public record Movie
{
    [Key]
    public int Id { get; init; }
    public required string Title { get; init; }

    /// <summary>
    /// Description of the movie in Dutch.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Description of the movie in English.
    /// </summary>
    public required string DescriptionEn { get; init; }
    public required int Duration { get; init; }
    public required DateOnly ReleaseDate { get; init; }
    public required Uri PosterUrl { get; init; }
    public required string Language { get; init; }
    public required int MinimumAge { get; init; }
    public required string Genre { get; init; }
    public required string Tags { get; init; }
}