using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
﻿using ConnectPlay.TicketPlay.Contracts.Overview;
using ConnectPlay.TicketPlay.UI.Api;
using Microsoft.AspNetCore.Components;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Pages;

public partial class MovieOverview : TranslatableComponent
{
    private readonly ILogger<MovieOverview> _logger;
    private readonly IWebsiteApi websiteApi;

    private IEnumerable<OverviewMovieDay> Overview = [];
    private bool isLoading = true; // the page starts in a loading state
    private string? errorMessage;

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
}