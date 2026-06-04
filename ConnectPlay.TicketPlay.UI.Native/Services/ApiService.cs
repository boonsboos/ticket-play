using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Contracts.Authentication;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;
using System.IdentityModel.Tokens.Jwt;

namespace ConnectPlay.TicketPlay.UI.Native.Services;

public class ApiService : IApiService, IHostedService, IDisposable
{
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private bool disposedValue;

    private Timer? _refreshTimer;
    private bool _offline = false;
    private bool _authenticated = false;

    private readonly IAuthApi _authApi;
    private readonly IMovieApi _movieApi;
    private readonly ISecureStorage _secureStorage;
    private readonly ILogger<ApiService> _logger;
    private readonly IConnectivity _connectivity;

    private const string TokenKey = "ticket_play_authtoken";
    private const string RefreshKey = "ticket_play_authrefresh";
    private const string ExpiresKey = "ticket_play_authlastrefresh";

    public ApiService(
        IAuthApi authApi,
        IMovieApi movieApi,
        ISecureStorage secureStorage,
        IConnectivity connectivity,
        ILogger<ApiService> logger)
    {
        this._authApi = authApi;
        this._movieApi = movieApi;
        this._secureStorage = secureStorage;
        this._logger = logger;
        this._connectivity = connectivity;
    }

    public bool IsAuthenticated => this._authenticated;
    public bool IsOffline => this._offline;

    public IEnumerable<Movie> FavouriteMovies { get; private set; } = [];

    public Task<string> GetTokenAsync() => this._secureStorage.GetAsync(TokenKey)!;

    public async Task<Guid> GetUserIdAsync()
    {
        var token = await GetTokenAsync();

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        return Guid.Parse(jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
    }

    public async Task LoginAsync(string email, string password)
    {
        await _refreshLock.WaitAsync();
        try
        {
            var response = await this._authApi.LoginAsync(new RegistrationRequest
            {
                Email = email,
                Password = password
            });

            if (!response.IsSuccessful || response.Content is null)
            {
                _logger.LogError("Failed to log in: {StatusCode} - {Error}", response.StatusCode, response.Error);
                this._authenticated = false;
                return;
            }

            _logger.LogInformation("Logged in succesfully");

            await this._secureStorage.SetAsync(TokenKey, $"Bearer {response.Content!.Token}");
            await this._secureStorage.SetAsync(RefreshKey, response.Content!.RefreshToken);
            await this._secureStorage.SetAsync(ExpiresKey, response.Content!.ExpiresIn.ToString());

            this._authenticated = true;
        } finally
        {
            _refreshLock.Release();
        }
    }

    public async Task<bool> RefreshAsync()
    {
        await _refreshLock.WaitAsync();
        try
        {
            var refreshToken = await this._secureStorage.GetAsync(RefreshKey) 
                ?? throw new InvalidOperationException("No valid refresh token found");

            var response = await this._authApi.RefreshAsync(new RefreshRequest { RefreshToken = refreshToken });
            if (!response.IsSuccessful || response.Content is null)
            {
                _logger.LogError("Failed to refresh token: {StatusCode} - {Error}", response.StatusCode, response.Error);
                this._authenticated = false;
                return false;
            }

            await this._secureStorage.SetAsync(TokenKey, response.Content!.Token);
            await this._secureStorage.SetAsync(RefreshKey, response.Content!.RefreshToken);
            await this._secureStorage.SetAsync(ExpiresKey, DateTime.UtcNow.AddSeconds(response.Content!.ExpiresIn).ToString());

            this._authenticated = true;

            return true;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task LogoutAsync()
    {
        this._authenticated = false;

        var token = await this._secureStorage.GetAsync(TokenKey);
        try
        {
            await this._authApi.LogoutAsync(token!);
        } catch (ApiException exc)
        {
            this._logger.LogError(exc, "Failed to log out");
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartAuthenticationLoopAsync();

        if (IsAuthenticated)
        {
            await FetchFavoriteMovies();
        }
    }

    public async Task RefreshAccountDataAsync()
    {
        if (!IsAuthenticated)
        {
            this._logger.LogError("Unable to refresh account data as the user is unauthenticated.");
            return;
        }

        await FetchFavoriteMovies();
    }

    private async Task FetchFavoriteMovies()
    {
        var response = await this._movieApi.GetFavoriteMoviesAsync(await this.GetTokenAsync());

        if (!response.IsSuccessStatusCode)
        {
            this._logger.LogError("Failed to fetch favorited movies for user: {Error}", response.Error);
        }

        this.FavouriteMovies = response.Content ?? [];
    }

    private async Task StartAuthenticationLoopAsync()
    {
        this._logger.LogDebug("Checking authentication status");

        // if we are not connected, check if our credentials are set
        if (this._connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            this._offline = true;

            var offlineRefreshToken = await this._secureStorage.GetAsync(RefreshKey);
            if (offlineRefreshToken is null)
            {
                // we can assume we have never been logged in
                this._logger.LogInformation("Application is offline and appears to not have been authenticated");
                this._authenticated = false;
            }
            else
            {
                // we assume that we are logged in
                this._logger.LogInformation("Application is offline, we cannot refresh the token. Assuming we have been logged in at some point");
                this._authenticated = true;
            }

            return;
        }

        // determine if we need to refresh
        var validRefresh = DateTime.TryParse(await this._secureStorage.GetAsync(ExpiresKey), out var refreshDue);

        // try refreshing current auth token
        if (validRefresh && DateTime.UtcNow > refreshDue)
        {
            if (!await TryRefresh())
            {
                this._logger.LogInformation("Failed to refresh because toke was null, user has to reauthenticate");
            }
            return;
        }
        else if (DateTime.UtcNow < refreshDue) // token is still valid 
        {
            this._logger.LogInformation("Token is still valid");

            var interval = await GetRefreshDueIntervalAsync();

            this._refreshTimer = new Timer(async _ => { await this.RefreshAsync(); }, null, 0, interval);

            this._authenticated = true;
            return;
        }

        this._logger.LogInformation("The user is no longer authenticated");

        // the user *has* to reauthenticate
        this._authenticated = false;
    }

    private async Task<bool> TryRefresh()
    {
        var refreshToken = await this._secureStorage.GetAsync(RefreshKey);
        if (refreshToken is not null && await this.RefreshAsync())
        {
            var interval = await GetRefreshDueIntervalAsync();

            this._refreshTimer = new Timer(async _ => { await this.RefreshAsync(); }, null, interval, interval);

            this._authenticated = true;
            return true;
        }

        return false;
    }

    private async Task<int> GetRefreshDueIntervalAsync()
    {
        var validRefresh = DateTime.TryParse(await this._secureStorage.GetAsync(ExpiresKey), out var refreshDue);

        return (int) DateTime.UtcNow.Subtract(refreshDue).TotalSeconds;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("ApiService stopping");
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _refreshLock.Dispose();
                _refreshTimer?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}