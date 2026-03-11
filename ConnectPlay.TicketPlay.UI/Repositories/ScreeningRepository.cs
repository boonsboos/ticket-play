using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Repositories;

public class ScreeningRepository : IScreeningRepository
{
    private readonly IScreeningApi _screeningApi;

    public ScreeningRepository(IScreeningApi screeningApi)
    {
        _screeningApi = screeningApi;
    }

    public async Task CreateScreeningAsync(CreateScreeningDto dto)
    {
        try
        {
            await _screeningApi.CreateScreeningAsync(dto);
        }
        catch (Refit.ApiException ex)
        {
            // Log of handleer API-fouten
            Console.WriteLine($"Error creating screening: {ex.Content}");
            throw;
        }
    }

    public Task<Screening?> GetScreeningAsync(int id)
    {
        try
        {
            // No API call available here yet — return null to satisfy interface.
            return Task.FromResult<Screening?>(null);
        }
        catch (Refit.ApiException ex)
        {
            Console.WriteLine($"Error fetching screening: {ex.Content}");
            return Task.FromResult<Screening?>(null);
        }
    }
}