using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.Contracts.Overview;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WebsiteController : ControllerBase
{
    private const string SneakPreviewUrl = "https://dummyimage.com/300x450/000/fff&text=Sneak%20Preview";

    private readonly IScreeningRepository screeningRepository;

    public WebsiteController(IScreeningRepository screeningRepository)
    {
        this.screeningRepository = screeningRepository;
    }

    [HttpGet]
    [Route("overview")]
    public async Task<IActionResult> GetOverviewAsync()
    {
        var screenings = await screeningRepository.GetWeekOverviewAsync();

        var overview = GetMovieOverview(screenings);

        return Ok(overview);
    }

    private static IEnumerable<OverviewMovieDay> GetMovieOverview(IEnumerable<Screening> screenings)
    {
        return screenings
            .GroupBy(screening => screening.StartTime.Date) // by day
            .Select(
                grouping => new OverviewMovieDay
                {
                    Day = new DateTimeOffset(grouping.Key),
                    Offerings = grouping
                        .GroupBy(day => day.Movie) // all movies on that day
                        .Select(dayScreenings => {
                            var apiMovie = new ApiMovie(dayScreenings.Key, dayScreenings);
                            
                            // handle sneak preview screenings
                            if (dayScreenings.Any(screening => screening.SneakPreview))
                            {
                                apiMovie = apiMovie with
                                {
                                    PosterUrl = SneakPreviewUrl,
                                    Title = "Sneak Preview",
                                };
                            }

                            return apiMovie;
                        })
                }
            ).ToList();
    }
}
