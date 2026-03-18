using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Controllers;
using Microsoft.AspNetCore.Mvc; // To use OkObjectResult and IActionResult
using NSubstitute; // This is the mocking library used in the test
using ConnectPlay.TicketPlay.Contracts.Movie;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class MovieControllerTests
{
    [TestMethod]
    public async Task GetToday_ReturnsOk_WhenNoMovies() // Method, Result and Scenario naming convention
    {
        // Arrange
        var movieRepository = Substitute.For<IMovieRepository>(); // Create fake repository

        // If the repository is called to get todays movies return an empty list
        movieRepository
            .GetTodaysMoviesAsync()
            .Returns(new List<OverviewMovie>()); // EMPTY LIST

        var controller = new MovieController(movieRepository); // Create controller with fake the repository

        // Act
        var todaysMovies = await controller.GetTodayAsync(); // Call the method we want to test

        // Assert
        var okResult = todaysMovies as OkObjectResult; // Try to cast the result to OkObjectResult

        Assert.IsNotNull(okResult); // if its null then the controller did not return an Ok result

        var movies = okResult.Value as IEnumerable<OverviewMovie>; // Cast the value of the okResult to a list of movies

        Assert.IsNotNull(movies); // if its null then the controller did not return a list of movies
        Assert.AreEqual(0, movies.Count()); // if the count is equal to 0 than the controller returned an empty list of movies

        await movieRepository.Received(1).GetTodaysMoviesAsync(); // Check if the repository only called once
    }
}