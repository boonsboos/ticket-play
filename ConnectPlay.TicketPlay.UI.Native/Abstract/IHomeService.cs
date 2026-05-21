using ConnectPlay.TicketPlay.Contracts.Overview;

namespace ConnectPlay.TicketPlay.UI.Native.Abstract;

public interface IHomeService
{
    Task<IEnumerable<OverviewMovieDay>> GetOverviewMoviesAsync();
}