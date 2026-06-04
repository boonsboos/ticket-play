using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.UI.Native.Abstract;

/// <summary>
/// Used for managing connectivity to the API. It proxies requests to their respective endpoints.
/// </summary>
public interface IApiService
{
    public bool IsAuthenticated { get; }
    public bool IsOffline { get; }

    public Task<string> GetTokenAsync();

    public Task<Guid> GetUserIdAsync();

    public IEnumerable<Movie> FavouriteMovies { get; }

    public Task LoginAsync(string email, string password);
    public Task<bool> RefreshAsync();
    public Task LogoutAsync();

    public Task RefreshAccountDataAsync();
}
