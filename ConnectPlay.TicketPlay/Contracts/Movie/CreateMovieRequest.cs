using System.ComponentModel.DataAnnotations;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Contracts.Movie;

public record CreateMovieRequest
{
    [StringLength(255)]
    public required string Title { get; init; }

    [StringLength(4000)]
    public required string Description { get; init; }

    [Range(1, 600)]
    public required int Duration { get; init; }

    public required DateOnly ReleaseDate { get; init; }

    public required Uri PosterUrl { get; init; }

    [StringLength(12)]
    public required string Language { get; init; }

    public required MinimumAgeRating MinimumAge { get; init; }

    [StringLength(50)]
    public required string Genre { get; init; }

    [MaxLength(500)]
    public List<string>? Tags { get; init; }
}
