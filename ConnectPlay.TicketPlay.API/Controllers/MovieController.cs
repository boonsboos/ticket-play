using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Contracts.Movie;
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
    [Route("all")] // At route /movie/all
    public async Task<IActionResult> GetAllAsync() // Always suffix async methods with "Async"
    {
        var movies = await _movieRepository.GetAllMoviesAsync();

        // HTTP OK
        return Ok(movies);
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

    [ProducesResponseType(typeof(IEnumerable<OverviewMovie>), StatusCodes.Status200OK)]
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest dto)
    {

        var allowedLanguages = new[] { "Nederlands", "English" };
        if (!allowedLanguages.Contains(dto.Language))
        {
            ModelState.AddModelError(nameof(dto.Language), "Language must be Nederlands or English.");
        }

        if (!Enum.IsDefined(typeof(MinimumAgeRating), dto.MinimumAge))
        {
            ModelState.AddModelError(nameof(dto.MinimumAge), "Invalid minimum age.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await _movieRepository.CreateMovieAsync(dto);
        return StatusCode(StatusCodes.Status201Created); // "Movie was created", no payload given.
    }
}
