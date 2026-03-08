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

    [ProducesResponseType(typeof(IEnumerable<MovieListItemDto>), StatusCodes.Status200OK)] // the response is a list of MovieListItemDto
    [HttpGet] // This is the Get endpoint.
    [Route("today")] // the route will become movie/today
    public async Task<IActionResult> GetTodayAsync() // Task<IActionResult> is the standard return type for async API endpoints (200 Ok, 404 Not Found)
    {
        var todaysMovies = await _movieRepository.GetTodaysMoviesAsync();

        return Ok(todaysMovies); // Ok() is short for OkObjectResult and creates a Http 200 response object with the data in it
    }

    [ProducesResponseType(typeof(IEnumerable<Movie>), StatusCodes.Status200OK)]
    [HttpGet]
    [Route("new")]
    public async Task<IActionResult> GetNewAsync()
    {
        // Returns a empty list if no movies are available
        var newMovies = await _movieRepository.GetNewMoviesAsync();
        return Ok(newMovies);
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Search([FromQuery] string movie, [FromBody] MovieFilters? filters)
    {
        return NotFound();
    }
}