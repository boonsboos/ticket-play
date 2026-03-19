using ConnectPlay.TicketPlay.Contracts.Hall;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IHallService
{

    /// <summary> 
    /// Gets the layout of a hall as a list of rows, where each row is a list of seats. Each seat is represented by an integer: 0 for a regular seat, 1 for a wheelchair seat.
    /// Returns null if the hall with the given ID does not exist.
    /// </summary>
    /// <param name="hallId">The ID of the hall.</param>
    /// <returns>A list of rows and the amount of seats in each row. Returns null if the hall with the given ID does not exist.</returns>
    public Task<HallLayoutResponse?> GetHallLayoutAsync(int hallId);
}
