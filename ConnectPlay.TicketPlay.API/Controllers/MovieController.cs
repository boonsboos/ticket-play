using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
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

    [ProducesResponseType(typeof(IEnumerable<Movie>), StatusCodes.Status200OK)]
    [HttpGet]
    [Route("new")]
    public Task<IActionResult> GetNew()
    {
        // HTTP No Content
        return Task.FromResult<IActionResult>(NoContent());
    }


    [HttpGet("{id:int}")]
    // Document that this endpoint returns a single movie on HTTP OK, and can also return HTTP Not Found if the movie with the given id does not exist.
    [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // The route parameter {id:int} means that this endpoint will match routes like /movie/123, and the id parameter will be parsed as an integer and passed to the method.
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        // Get the movie with the given id from the repository. This will return null if no such movie exists.
        var movie = await _movieRepository.GetMovieByIdAsync(id);

        if (movie == null)
            return NotFound();

        return Ok(movie);
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Search([FromQuery] string movie, [FromBody] MovieFilters? filters)
    {
        return NotFound();
    }
}