using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IHallApi
{
    [Post("/hall")]
    public Task<ApiResponse<CreateHallResponse>> CreateNewHallAsync(CreateHallRequest hallRequest);
    [Get("/hall/all")]
    Task<IEnumerable<Hall>> GetHallsAsync();
    [Get("/hall/{id}/layout")]
    Task<ApiResponse<HallLayoutResponse?>> GetHallLayoutAsync(int id);
}