using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IFavoritesRepository
{
    Task AddFavoriteAsync(Guid userId, int movieId);
    Task RemoveFavoriteAsync(Guid userId, int movieId);
    Task<IEnumerable<Movie>> GetFavoritesAsync(Guid userId);
}
