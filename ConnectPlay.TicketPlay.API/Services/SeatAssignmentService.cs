using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Services;

public class SeatAssignmentService : ISeatAssignmentService
{
    private readonly ISeatRepository seatRepository;
    private readonly ITicketRepository ticketRepository;

    public SeatAssignmentService(ISeatRepository seatRepository, ITicketRepository ticketRepository)
    {
        this.seatRepository = seatRepository;
        this.ticketRepository = ticketRepository;
    }

    public async Task<IEnumerable<Seat>> AssignAsync(Screening screening, IEnumerable<TicketType> tickets)
    {
        var availableSeats = await GetAvailableSeatsAsync(screening);

        if (availableSeats.Count < tickets.Count())
        {
            throw new InvalidOperationException($"A request was made for {tickets.Count()} tickets, but the screening only has room for {availableSeats.Count} people");
        }

        var rows = availableSeats.GroupBy(seat => seat.Row).ToList();

        rows = SortFromMiddle(rows);

        var assignedSeats = new List<Seat>();

        // 1) try to fit as many people next to each other
        assignedSeats.AddRange(InternalAssignSeatsOptimistic(rows, tickets));

        // 2) spread the seats through the row otherwise
        if (assignedSeats.Count == 0)
        {
            assignedSeats.AddRange(InternalAssignSeatsPessimistic(rows, tickets));
        }

        // 3) spread the seats throughout the entire hall
        if (assignedSeats.Count == 0)
        {
            assignedSeats.AddRange(availableSeats.Take(tickets.Count()));
        }

        return assignedSeats;
    }

    private async Task<List<Seat>> GetAvailableSeatsAsync(Screening screening)
    {
        var soldTickets = await ticketRepository.GetTicketsAsync(screening);
        var soldSeats = soldTickets.Select(ticket => ticket.Seat).ToList();

        var totalSeats = await seatRepository.GetSeatsAsync(screening.Hall);

        return totalSeats.Except(soldSeats).ToList();
    }

    /// <summary>
    /// Assigns seats where all visitors can sit next to each other
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="tickets"></param>
    /// <returns></returns>
    private List<Seat> InternalAssignSeatsOptimistic(IEnumerable<IGrouping<int, Seat>> rows, IEnumerable<TicketType> tickets)
    {
        var amountOfSeats = tickets.Count();

        var firstFittingRow = rows.FirstOrDefault(row => HasAdjacentSeats(row, amountOfSeats));
        if (firstFittingRow is null) return [];

        return [.. firstFittingRow.Take(amountOfSeats)];
    }

    /// <summary>
    /// Assigns seats where all visitors can not sit next to each other but still on the same row
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="tickets"></param>
    /// <returns></returns>
    private List<Seat> InternalAssignSeatsPessimistic(IEnumerable<IGrouping<int, Seat>> rows, IEnumerable<TicketType> tickets)
    {
        var amountOfSeats = tickets.Count();
        var firstFittingRow = rows.FirstOrDefault(row => row.Count() >= amountOfSeats);
        if (firstFittingRow is null) return [];

        return [.. firstFittingRow.Take(amountOfSeats)];
    }

    /// <summary>
    /// Sort a list from the middle out
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">the list to sort</param>
    private static List<T> SortFromMiddle<T>(List<T> list)
    {
        var length = list.Count();
        var oddLength = length % 2 == 1;

        List<T> items = [];

        int middle = length / 2;
        int leftSide, rightSide;

        if (oddLength)
        {
            leftSide = middle - 1;
            rightSide = middle + 1;
            items.Add(list[middle]);
        }
        else
        {
            leftSide = middle - 1;
            rightSide = middle;
        }

        while (leftSide != -1 && rightSide < length)
        {
            items.Add(list[leftSide]);
            items.Add(list[rightSide]);
            leftSide--;
            rightSide++;
        }

        return items;
    }

    /// <summary>
    /// Checks if a row has count amount of adjacent seats
    /// </summary>
    /// <param name="row"></param>
    /// <param name="count"></param>
    /// <returns>true if it has the amount of adjacent seats</returns>
    private static bool HasAdjacentSeats(IEnumerable<Seat> row, int count)
    {
        if (row.Count() == 1 && count == 1) return true;

        var seatNumbers = row.Select(seat => seat.SeatNumber);
        var start = seatNumbers.First();
        var amount = 1;

        foreach (var seatNumber in seatNumbers)
        {
            if (seatNumber - 1 == start)
            {
                amount++;
                start = seatNumber;
            }

            if (amount == count) return true;
        }

        return amount >= count;
    }
}