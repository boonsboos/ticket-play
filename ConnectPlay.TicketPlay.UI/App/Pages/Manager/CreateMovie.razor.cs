using System.ComponentModel.DataAnnotations;
using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.App.Components.Base;

namespace ConnectPlay.TicketPlay.UI.App.Pages.Manager;

public partial class CreateMovie(IMovieApi movieApi) : TranslatableComponent
{
    protected CreateMovieFormModel model = new();

    protected bool isSubmitting;
    protected string? successMessage;
    protected string? errorMessage;

    protected async Task HandleValidSubmit()
    {
        isSubmitting = true;
        successMessage = null;
        errorMessage = null;

        try
        {
            if (!Uri.TryCreate(model.PosterUrl, UriKind.Absolute, out var posterUri))
            {
                errorMessage = "Poster URL must be a valid absolute URL.";
                return;
            }

            var tags = ParseTags(model.TagsCsv);

            var dto = new CreateMovieRequest
            {
                Title = model.Title!,
                Description = model.Description!,
                Duration = model.Duration!.Value,
                ReleaseDate = DateOnly.FromDateTime(model.ReleaseDate!.Value),
                PosterUrl = posterUri,
                Language = model.Language!,
                MinimumAge = (int)model.MinimumAge!.Value,
                Genre = model.Genre!,
                Tags = tags
            };

            await movieApi.CreateMovieAsync(dto);

            successMessage = "Movie has been created successfully";
            model = new();
        }
        catch (Exception ex)
        {
            errorMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
        }
    }

    protected static List<string> ParseTags(string? csv)
        => (csv ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    protected sealed class CreateMovieFormModel
    {
        [Required, StringLength(255)]
        public string? Title { get; set; }

        [Required, StringLength(4000)]
        public string? Description { get; set; }

        [Required, Range(1, 600)]
        public int? Duration { get; set; }

        [Required]
        public DateTime? ReleaseDate { get; set; } = DateTime.Today;

        [Required]
        public string? PosterUrl { get; set; }

        [Required]
        [StringLength(12)]
        public string? Language { get; set; }

        [Required]
        [Range(0, 18)]
        public MinimumAgeRating? MinimumAge { get; set; }

        [Required, StringLength(50)]
        public string? Genre { get; set; }

        [StringLength(500)]
        public string? TagsCsv { get; set; }
    }
}