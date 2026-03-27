using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Arrangement;
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
    private readonly IArrangementRepository arrangementRepository;

    public WebsiteController(IScreeningRepository screeningRepository, IArrangementRepository arrangementRepository)
    {
        this.screeningRepository = screeningRepository;
        this.arrangementRepository = arrangementRepository;
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
                            var apiMovie = new OverviewMovie()
                            {
                                Id = dayScreenings.Key.Id.ToString(),
                                Title = dayScreenings.Key.Title,
                                Genre = dayScreenings.Key.Genre,
                                PosterUrl = dayScreenings.Key.PosterUrl.AbsoluteUri,
                                ScreeningTimes = screenings.Select(screening => screening.StartTime)
                            };
                            
                            // handle sneak preview screenings
                            if (dayScreenings.Any(screening => screening.SneakPreview))
                            {
                                apiMovie = apiMovie with
                                {
                                    Id = "sneakpreview",
                                    PosterUrl = SneakPreviewUrl,
                                    Title = "Sneak Preview",
                                };
                            }

                            return apiMovie;
                        })
                }
            ).ToList();
    }

    [HttpPost]
    [Route("arrangements")]
    public async Task<IActionResult> CreateArrangementAsync([FromBody] NewArrangement newArrangement)
    {
        await arrangementRepository.CreateAsync(newArrangement);

        return Ok();
    }

    [HttpGet]
    [Route("arrangements")]
    public async Task<IActionResult> GetArrangementsAsync()
    {
        var arrangements = await arrangementRepository.GetAllAsync();

        return Ok(arrangements);
    }
}
