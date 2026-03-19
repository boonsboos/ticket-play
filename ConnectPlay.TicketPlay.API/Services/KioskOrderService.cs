using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;
using System.Data;

namespace ConnectPlay.TicketPlay.API.Services;

public class KioskOrderService(
        IScreeningRepository screeningRepository,
        ITicketRepository ticketRepository,
        ISeatAssignmentService seatAssignmentService,
        IPriceCalculationService priceCalculationService,
        ITicketPrintingService ticketPrintingService,
        IOrderRepository orderRepository,
        ISeatRepository seatRepository,
        ILogger<KioskOrderService> logger) : IKioskOrderService
{

    public async Task<Order> ReserveAsync(int screeningId, IEnumerable<TicketType> reservation)
    {
        // find if screening is real
        var screening = await screeningRepository.GetScreeningAsync(screeningId)
            ?? throw new ArgumentException("Screening does not exist");

        // calculate cost
        var total = priceCalculationService.CalculatePrices(screening, reservation);

        // assign seats
        var assignedSeats = await seatAssignmentService.AssignAsync(screening, reservation);

        var tickets = await CreateTicketsAsync(screening, reservation, assignedSeats);

        // create the order object first (tickets are empty for now)
        var order = new Order
        {
            Total = total,
            Status = OrderStatus.Pending,
            // save the order associated with the tickets
            Tickets = tickets,
        };

        await orderRepository.CreateOrderAsync(order);
        return await orderRepository.GetOrderByIdAsync(order.Id) ?? throw new InvalidOperationException("Failed to retrieve order after creation");
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

        var tickets = order.Tickets.ToList();

        if (seats.Count() != tickets.Count)
        {
            throw new InvalidOperationException("The number of seats must match the number of tickets in the order");
        }

        // Get the screening to access hall information
        var screening = await screeningRepository.GetScreeningAsync(tickets[0].ScreeningId)
            ?? throw new InvalidOperationException("Screening not found for order tickets");

        for (int i = 0; i < tickets.Count; i++)
        {
            var seat = await seatRepository.GetSeatByRowAndNumberAsync(screening.Hall.Id, seats.ElementAt(i).Row, seats.ElementAt(i).SeatNumber, seats.ElementAt(i).IsForWheelchair)
                ?? throw new InvalidOperationException($"Seat not found for row {seats.ElementAt(i).Row} and seat number {seats.ElementAt(i).SeatNumber}");

            tickets[i].SeatId = seat.Id;
            tickets[i].Seat = seat;
        }
        logger.LogInformation("Updating seats for order {OrderId} with [{seatsCount}]", orderId, tickets.Select(t => $"row {t.Seat.Row} seat {t.Seat.SeatNumber} (seatId: {t.SeatId})").Aggregate((a, b) => a + ", " + b));

        await ticketRepository.UpdateTicketsAsync(tickets);

        return await orderRepository.GetOrderByIdAsync(orderId) ?? throw new InvalidOperationException("Failed to retrieve order after updating seats");
    }
}