using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IHallApi
{
    [Post("/hall")]
    public Task<Hall> CreateNewHallAsync(CreateHallRequest hallRequest);
}