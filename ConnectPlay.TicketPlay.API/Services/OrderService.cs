using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Abstract.Services;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;
using System.Data;

namespace ConnectPlay.TicketPlay.API.Services;

public class OrderService(
        IScreeningRepository screeningRepository,
        ITicketRepository ticketRepository,
        IArrangementRepository arrangementRepository,
        ISeatAssignmentService seatAssignmentService,
        IPriceCalculationService priceCalculationService,
        ITicketPrintingService ticketPrintingService,
        IOrderRepository orderRepository,
        ISeatRepository seatRepository,
        IOrderArrangementRepository orderArrangementRepository,
        ILogger<OrderService> logger) : IOrderService
{

    public async Task<Order> ReserveAsync(int screeningId, NewOrder newOrder)
    {
        // find if screening is real
        var screening = await screeningRepository.GetScreeningAsync(screeningId)
            ?? throw new ArgumentException("Screening does not exist");

        // ensure the user did not tamper with the arrangements, otherwise we cannot proceed with the order.
        foreach (var arrangementQuantity in newOrder.Arrangements)
        {
            arrangementQuantity.Arrangement = await arrangementRepository.GetByIdAsync(arrangementQuantity.Arrangement.Id) 
                ?? throw new ArgumentException($"Arrangement {arrangementQuantity.Arrangement.Id} does not exist");
        }

        // calculate cost, including arrangements
        var total = priceCalculationService.CalculatePrices(screening, newOrder);

        // assign seats
        var assignedSeats = await seatAssignmentService.AssignAsync(screening, newOrder.Tickets);

        var tickets = await CreateTicketsAsync(screening, newOrder.Tickets, assignedSeats);

        // create the order object first (tickets are empty for now)
        var order = new Order
        {
            Total = total,
            Status = OrderStatus.Pending,
            Tickets = tickets,
        };

        // save the order associated with the tickets
        await orderRepository.CreateOrderAsync(order);

        await orderArrangementRepository.SaveArrangementsAsync(order, newOrder.Arrangements);

        // fetch it again to make sure we have the most recent data
        return await orderRepository.GetOrderByIdAsync(order.Id)
            ?? throw new InvalidOperationException("Failed to retrieve order after creation");
    }

    // We use await because the methods are async and we want to ensuse that the order is only cancelled
    // after the tickets are deltere and the order is updated
    public async Task CancelAsync(int orderId)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId) ?? throw new ArgumentException("Order does not exist"); // get the order so we can chagne te status of the order

        order.Status = OrderStatus.Cancelled; // change the status of the order to cancelled

        await ticketRepository.DeleteTicketsByOrderIdAsync(orderId);

        await orderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
    }

    private static async Task<List<Ticket>> CreateTicketsAsync(Screening screening, IEnumerable<TicketType> reservation, IEnumerable<Seat> assignedSeats)
    {
        List<Ticket> tickets = [];
        for (int i = 0; i < reservation.Count(); i++)
        {
            var t = new Ticket
            {
                ScreeningId = screening.Id,
                SeatId = assignedSeats.ElementAt(i).Id,
                TicketType = reservation.ElementAt(i),
                OrderId = null,
            };
            tickets.Add(t);
        }

        return tickets;
    }

    public async Task PayAsync(int orderId)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("Order does not exist");

        if (order.Status == OrderStatus.Paid)
        {
            logger.LogInformation("Order {OrderId} has already been paid", orderId);
            return;
        }

        await orderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Paid);
    }

    public async Task<Stream> PrintAsync(int orderId)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("Order does not exist");

        if (order.Status == OrderStatus.Redeemed)
        {
            throw new InvalidOperationException($"Order {orderId} has already been redeemed");
        }

        // if order has already been redeemed, do not let the user print the tickets again
        if (order.Status != OrderStatus.Paid)
        {
            throw new InvalidOperationException($"Order {orderId} has not been paid yet");
        }

        logger.LogInformation("Updating status of order {OrderId} to Redeemed", orderId);

        await orderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Redeemed);

        return await ticketPrintingService.PrintTicketsAsync(order);
    }

    /// <summary>
    /// Gets the taken seats for a screening.
    /// <para>We need the orderId to exclude the current order from the taken seats, otherwise the user would not see any available seats after reserving because their own reserved seats would also be shown as taken.</para>
    /// <para>We only want to show the taken seats from orders with a status of Paid, because only those seats are actually taken.
    /// Orders with a status of Pending should not be included because those seats are not yet taken and can still be selected by other users. Orders with a status of Cancelled should also not be included because those seats are also not taken.</para>
    /// </summary>
    /// <param name="screeningId"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<IEnumerable<SeatResponse>> GetTakenSeatsAsync(int screeningId, int orderId)
    {
        var screening = await screeningRepository.GetScreeningAsync(screeningId)
            ?? throw new ArgumentException("Screening does not exist");

        var currentOrder = await orderRepository.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("Order does not exist");

        var tickets = await ticketRepository.GetTicketsByScreeningIdAsync(screeningId);

        var takenSeats = tickets
            .Where(t => t.OrderId != orderId)
            .Select(ticket => new SeatResponse
            {
                Row = ticket.Seat.Row,
                SeatNumber = ticket.Seat.SeatNumber,
                IsForWheelchair = ticket.Seat.IsForWheelchair,
                IsReserved = ticket.Order?.Status == OrderStatus.Pending,
                IsTaken = ticket.Order?.Status == OrderStatus.Paid,
            })
            .ToList();

        return takenSeats;
    }

    public async Task<Order> UpdateSeatsAsync(int orderId, IEnumerable<Seat> seats)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("Order does not exist");

        if (order.Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Only orders with status Pending can be updated");
        }

        var seatList = seats.ToList();
        var tickets = order.Tickets.ToList();

        if (seatList.Count != tickets.Count)
        {
            throw new InvalidOperationException("The number of seats must match the number of tickets in the order");
        }

        // Reject duplicate seat selections within the request
        var distinctSeats = seatList.Select(s => (s.Row, s.SeatNumber, s.IsForWheelchair)).Distinct().ToList();
        if (distinctSeats.Count != seatList.Count)
        {
            throw new InvalidOperationException("Duplicate seat selections are not allowed");
        }

        // Get the screening to access hall information
        var screening = await screeningRepository.GetScreeningAsync(tickets[0].ScreeningId)
            ?? throw new InvalidOperationException("Screening not found for order tickets");

        // Resolve all seat entities from the database first
        var resolvedSeats = new List<Seat>();
        for (int i = 0; i < seatList.Count; i++)
        {
            var seat = await seatRepository.GetSeatByRowAndNumberAsync(screening.Hall.Id, seatList[i].Row, seatList[i].SeatNumber, seatList[i].IsForWheelchair)
                ?? throw new InvalidOperationException($"Seat not found for row {seatList[i].Row} and seat number {seatList[i].SeatNumber}");
            resolvedSeats.Add(seat);
        }

        // Check that none of the requested seats are already taken or reserved by other orders for the same screening
        var existingTickets = await ticketRepository.GetTicketsByScreeningIdAsync(tickets[0].ScreeningId);
        var unavailableSeatIds = existingTickets
            .Where(t => t.OrderId != orderId && (t.Order?.Status == OrderStatus.Paid || t.Order?.Status == OrderStatus.Pending))
            .Select(t => t.SeatId)
            .ToHashSet();

        var conflictingSeats = resolvedSeats.Where(s => unavailableSeatIds.Contains(s.Id)).ToList();
        if (conflictingSeats.Count > 0)
        {
            var seatDescriptions = string.Join(", ", conflictingSeats.Select(s => $"row {s.Row} seat {s.SeatNumber}"));
            throw new InvalidOperationException($"The following seats are not available: {seatDescriptions}");
        }

        for (int i = 0; i < tickets.Count; i++)
        {
            tickets[i].SeatId = resolvedSeats[i].Id;
            tickets[i].Seat = resolvedSeats[i];
        }
        logger.LogInformation("Updating seats for order {OrderId} with [{seatsCount}]", orderId, tickets.Select(t => $"row {t.Seat.Row} seat {t.Seat.SeatNumber} (seatId: {t.SeatId})").Aggregate((a, b) => a + ", " + b));

        await ticketRepository.UpdateTicketsAsync(tickets);

        return await orderRepository.GetOrderByIdAsync(orderId) ?? throw new InvalidOperationException("Failed to retrieve order after updating seats");
    }
}