using Microsoft.AspNetCore.Components;
using ConnectPlay.TicketPlay.Models.Dto;
using ConnectPlay.TicketPlay.Abstract.Repositories;
using System.Threading.Tasks;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class MovieDetail : ComponentBase
{
    [Parameter] public int Id { get; init; }

    [Inject] protected IMovieRepository MovieRepository { get; init; } = default!;

    protected MovieDetailDto? Movie { get; set; }

    private bool _hasLoaded = false;

    protected override async Task OnParametersSetAsync()
    {
        if (!_hasLoaded && Id != 0)
        {
            _hasLoaded = true;
            try
            {
                Movie = await MovieRepository.GetMovieByIdAsync(Id);
            }
            catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Movie = null;
                Console.WriteLine($"Movie with Id {Id} not found.");
            }
            catch (Exception ex)
            {
                Movie = null;
                Console.WriteLine($"Error fetching movie {Id}: {ex.Message}");
            }
        }
    }
}