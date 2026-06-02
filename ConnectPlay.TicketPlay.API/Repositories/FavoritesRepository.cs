using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class FavoritesRepository : IFavoritesRepository
{
    private readonly IDbContextFactory<TicketPlayContext> _dbContextFactory;

    public FavoritesRepository(IDbContextFactory<TicketPlayContext> dbContextFactory)
    {
        this._dbContextFactory = dbContextFactory;
    }

    public async Task AddFavoriteAsync(Guid userId, int movieId)
    {
        await using var dbContext = await this._dbContextFactory.CreateDbContextAsync();

        var movie = await dbContext.Movies.Where(m => m.Id == movieId).FirstOrDefaultAsync()
            ?? throw new ArgumentOutOfRangeException($"Movie {movieId} does not exist");

        var user = await dbContext.Users.Where(u =>  u.Id == userId).FirstOrDefaultAsync()
            ?? throw new ArgumentOutOfRangeException($"User {userId} does not exist anymore while adding a favorite?");

        if (user.Favorites.Contains(movie)) {
            return;
        }

        user.Favorites.Add(movie);

        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Movie>> GetFavoritesAsync(Guid userId)
    {
        await using var dbContext = await this._dbContextFactory.CreateDbContextAsync();

        var user = await dbContext.Users.Include(u => u.Favorites).Where(u => u.Id == userId).FirstOrDefaultAsync()
            ?? throw new ArgumentOutOfRangeException($"User {userId} does not exist anymore while fetching favorites?");

        return user.Favorites;
    }

    public async Task RemoveFavoriteAsync(Guid userId, int movieId)
    {
        await using var dbContext = await this._dbContextFactory.CreateDbContextAsync();

        var movie = await dbContext.Movies.Where(m => m.Id == movieId).FirstOrDefaultAsync()
            ?? throw new ArgumentOutOfRangeException($"Movie {movieId} does not exist");

        var user = await dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync()
            ?? throw new ArgumentOutOfRangeException($"User {userId} does not exist anymore while removing favorites?");

        user.Favorites.Remove(movie);

        await dbContext.SaveChangesAsync();
    }
}
