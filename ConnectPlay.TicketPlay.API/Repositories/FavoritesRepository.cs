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

        if (user.Favorites.Any(f => f.Movie.Id == movieId)) {
            return;
        }

        dbContext.Favorites.Add(new Favorite
        {
            Id = Guid.NewGuid(),
            Movie = movie,
            User = user
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Movie>> GetFavoritesAsync(Guid userId)
    {
        await using var dbContext = await this._dbContextFactory.CreateDbContextAsync();

        return await dbContext.Favorites
            .Include(f => f.Movie)
            .Include(f => f.User)
            .Where(f => f.User.Id == userId)
            .Select(f => f.Movie)
            .ToListAsync();
    }

    public async Task RemoveFavoriteAsync(Guid userId, int movieId)
    {
        await using var dbContext = await this._dbContextFactory.CreateDbContextAsync();

        var movie = await dbContext.Movies.Where(m => m.Id == movieId).FirstOrDefaultAsync()
            ?? throw new ArgumentOutOfRangeException($"Movie {movieId} does not exist");

        var user = await dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync()
            ?? throw new ArgumentOutOfRangeException($"User {userId} does not exist anymore while removing favorites?");

        var fav = user.Favorites.First(f => f.Movie.Id == movie.Id && f.User.Id == user.Id);

        user.Favorites.Remove(fav);

        await dbContext.SaveChangesAsync();
    }
}
