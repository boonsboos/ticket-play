using ConnectPlay.TicketPlay.Contracts.Hall;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IHallApi
{
    [Post("/hall")]
    public Task<ApiResponse<CreateHallResponse>> CreateNewHallAsync(CreateHallRequest hallRequest);
}