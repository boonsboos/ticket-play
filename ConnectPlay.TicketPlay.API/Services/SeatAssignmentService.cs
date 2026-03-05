using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto.Ticket;

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

    public async Task<IEnumerable<Seat>> AssignAsync(Screening screening, IEnumerable<CreateTicketDto> tickets)
    {
        var availableSeats = await GetAvailableSeatsAsync(screening);

        var rows = availableSeats.GroupBy(seat => seat.Row).ToList();

        SortFromMiddle(rows);

        var assignedSeats = new List<Seat>();

        // Actual seat assignment process:
        // find the row with the most optimal placing for the seats
        // but start from the middle and prefer the left

        // 1) try to fit as many people next to each other
        assignedSeats.AddRange(InternalAssignSeatsOptimistic(rows, tickets));

        // 2) spread the seats through the hall
        if (assignedSeats.Count() < tickets.Count())
        {
            assignedSeats.AddRange(InternalAssignSeatsPessimistic(rows, tickets));
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
    private List<Seat> InternalAssignSeatsOptimistic(IEnumerable<IGrouping<int, Seat>> rows, IEnumerable<CreateTicketDto> tickets)
    {
        var amountOfSeats = tickets.Count();

        var firstFittingRow = rows.FirstOrDefault(row => HasAdjacentSeats(row, amountOfSeats));
        if (firstFittingRow is null) return [];

        return [.. firstFittingRow];
    }

    /// <summary>
    /// Assigns seats where all visitors can not sit next to each other
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="tickets"></param>
    /// <returns></returns>
    private List<Seat> InternalAssignSeatsPessimistic(IEnumerable<IGrouping<int, Seat>> rows, IEnumerable<CreateTicketDto> tickets)
    {
        return [];
    }

    /// <summary>
    /// Sort a list from the middle out
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">the list to sort</param>
    private static void SortFromMiddle<T>(List<T> list)
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
        } else
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
