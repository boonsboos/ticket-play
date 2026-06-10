using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.Api;

public interface IRecommendationApi
{
    [Get("/recommendations")]
    public Task<ApiResponse<IEnumerable<Movie>>> GetRecommendations([Header("Authorization")] string jwt);
}
