using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

// API resource for movie related queries.

[ApiController] // Register as part of the API
[Route("[controller]")] // Base route contains the name of the controller (MovieController -> movie)
public class MovieController : ControllerBase // Controllerbase provides useful utility methods for returning HTTP responses
{
    private readonly IMovieRepository _movieRepository;

    public MovieController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    // ProducesResponseType is not required, but is useful for API documentation and tools like Swagger to understand what this endpoint returns.
    [ProducesResponseType(typeof(IEnumerable<Movie>), StatusCodes.Status200OK)] // Document that this endpoint returns a list of movies on HTTP OK
    [HttpGet] // Register a GET endpoint
    [Route("current")] // At route /movie/current
    public async Task<IActionResult> GetCurrentAsync() // Always suffix async methods with "Async"
    {
        var currentMovies = await _movieRepository.GetCurrentMoviesAsync();

        // HTTP OK
        return Ok(currentMovies);
    }

    [ProducesResponseType(typeof(IEnumerable<MovieListItemDto>), StatusCodes.Status200OK)]
    [HttpGet] // This is the Get endpoint.
    [Route("today")]
    public async Task<IActionResult> GetTodayAsync()
    {
        var now = DateTimeOffset.Now; // We take the current date and time to find movies that are playing today
        var result = await _movieRepository.GetTodaysMoviesAsync(now);
        return Ok(result);
    }

    [ProducesResponseType(typeof(IEnumerable<Movie>), StatusCodes.Status200OK)]
    [HttpGet]
    [Route("new")]
    public Task<IActionResult> GetNew()
    {
        // HTTP No Content
        return Task.FromResult<IActionResult>(NoContent());
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Search([FromQuery] string movie, [FromBody] MovieFilters? filters)
    {
        return NotFound();
    }
}