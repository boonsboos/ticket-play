using ConnectPlay.TicketPlay.Contracts.Hall;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IHallService
{

    /// <summary> 
    /// Gets the layout of a hall as a list of rows, where each row is represented by the number of seats in that row.
    /// The returned <see cref="HallLayoutResponse"/> also includes information about wheelchair seats in the hall.
    /// Returns null if the hall with the given ID does not exist.
    /// </summary>
    /// <param name="hallId">The ID of the hall.</param>
    /// <returns>A <see cref="HallLayoutResponse"/> containing the seat count for each row and wheelchair seat information, or null if the hall with the given ID does not exist.</returns>
    public Task<HallLayoutResponse?> GetHallLayoutAsync(int hallId);
}
