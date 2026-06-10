using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages;

public partial class You : ComponentBase
{
    private readonly IApiService _apiService;
    private readonly IRecommendationApi _recommendationApi;
    private readonly ILogger<You> _logger;

    private IEnumerable<Movie> Favorites { get; set; } = [];

    private IEnumerable<Movie> Recommendations { get; set; } = [];

    public You(IApiService apiService, IRecommendationApi recommendationApi, ILogger<You> logger)
    {
        this._apiService = apiService;
        this._recommendationApi = recommendationApi;
        this._logger = logger;
    }

    protected override async Task OnInitializedAsync()
    {
        await _apiService.RefreshAccountDataAsync();

        Favorites = _apiService.FavouriteMovies;

        var token = await _apiService.GetTokenAsync();

        var response = await _recommendationApi.GetRecommendations(token);

        if (!response.IsSuccessStatusCode || response.Content is null)
        {
            _logger.LogInformation("Failed to get recommendations: {StatusCode} - {Error}", response.StatusCode, response.Error);
            return;
        }

        Recommendations = response.Content;
    }
}
