using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IRecommendationService
{
    public Task<IEnumerable<Movie>> RecommendMoviesAsync(Guid userId); 
}