using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Movie;
﻿using ConnectPlay.TicketPlay.Contracts.Overview;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Pages;

public partial class MovieOverview : TranslatableComponent
{
    private readonly ILogger<MovieOverview> _logger;
    private readonly IWebsiteApi websiteApi;

    private IEnumerable<OverviewMovieDay> Overview = [];
    private bool isLoading = true; // the page starts in a loading state
    private string? errorMessage;

    private string? searchTerm;
    private string? selectedGenre;
    private int? selectedMinimumAge;
    private string? selectedFormat;

    public MovieOverview(ILogger<MovieOverview> logger, IWebsiteApi websiteApi)
    {
        _logger = logger;
        this.websiteApi = websiteApi;
    }

    protected override async Task OnInitializedAsync() // Starts when the page is initialized
    {
        try
        {
           Overview = await websiteApi.GetWeekOverviewAsync(); // Get the overview of the week from the API
        }
        catch (ApiException e)
        {
            errorMessage = "home.errorLoadingMovies";
            _logger.LogError(e, "Error when loading the movies of today from the API.");
        }
        finally // This will always run
        {
            isLoading = false;
        }
    }

    /// <summary>
    /// Adds the selected filters, search, genre, minimum age and 2D-3D
    /// to of the given movies and returns filteredMovies result sorted by title
    /// </summary>
    private IEnumerable<OverviewMovie> AddFilter(IEnumerable<OverviewMovie> movies)
    {
        // create a new list of movies so the we can apply the fillters on it without changing the original list
        var filteredMovies = movies;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // for each movie in the list check if the title contains the search term            
            filteredMovies = filteredMovies.Where(movie => movie.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(selectedGenre))
        {
            filteredMovies = filteredMovies.Where(movie => movie.Genre == selectedGenre);
        }

        if (selectedMinimumAge.HasValue)
        {
            filteredMovies = filteredMovies.Where(movie => movie.MinimumAge == selectedMinimumAge.Value);
        }

        if (!string.IsNullOrWhiteSpace(selectedFormat))
        {
            filteredMovies = selectedFormat switch
            {
                "3D" => filteredMovies.Where(movie => movie.Is3D),
                "2D" => filteredMovies.Where(movie => !movie.Is3D),
                _ => filteredMovies
            };
        }

        return filteredMovies.OrderBy(movie => movie.Title);
    }

    /// <summary>
    /// Get all genres from the overview (all movies of all days) and return a distinct list of genres sorted alphabetically
    /// Overview 
    /// </summary>
    private IEnumerable<string> GetGenres()
    {
        
        return Overview
            .SelectMany(day => day.Offerings) // Get all movies from all days and put them in one list
            .Select(movie => movie.Genre)
            .Distinct() // Remove duplicates
            .OrderBy(genre => genre);
    }

    /// <summary>
    /// Get all minimum ages from the overview (all movies of all days) and return a distinct list of minimum ages sorted ascending
    /// </summary>
    private IEnumerable<int> GetAges()
    {
        return Overview
            .SelectMany(day => day.Offerings)
            .Select(movie => movie.MinimumAge)
            .Distinct()
            .OrderBy(age => age);
    }
}