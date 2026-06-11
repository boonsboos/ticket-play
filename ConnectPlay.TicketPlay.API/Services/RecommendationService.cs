using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Services;

public class RecommendationService : IRecommendationService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IFavoritesRepository _favoritesRepository;

    public RecommendationService(IMovieRepository movieRepository, IFavoritesRepository favoritesRepository)
    {
        this._movieRepository = movieRepository;
        this._favoritesRepository = favoritesRepository;
    }

    public async Task<IEnumerable<Movie>> RecommendMoviesAsync(Guid userId)
    {
        // get the favorites
        var favorites = await _favoritesRepository.GetFavoritesAsync(userId);

        if (!favorites.Any())
            return [];

        // get the tags, removing duplicates
        var favoriteTags = favorites.SelectMany(movie => movie.Tags.Split(",")).ToHashSet();

        // find movies matching those tags, removing duplicates
        var moviesPerTag = await Task.WhenAll(favoriteTags.Select(async tag => await _movieRepository.GetMoviesWithTagAsync(tag)));

        var movies = moviesPerTag.SelectMany(mpt => mpt);
            
        // remove the favorites from the tags and take the first 10
        return [.. movies.Except(favorites).Take(10)];
    }
}
