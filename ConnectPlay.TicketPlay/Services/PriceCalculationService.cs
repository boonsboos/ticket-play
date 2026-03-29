using ConnectPlay.TicketPlay.Abstract.Services;
using ConnectPlay.TicketPlay.Extensions;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Services;

public class PriceCalculationService : IPriceCalculationService
{
    public decimal CalculatePrices(Screening screening, NewOrder newOrder)
    {
        var total = 0m;

        foreach (TicketType ticketType in newOrder.Tickets)
        {
            total += CalculatePrice(screening, ticketType);
        }

        // authenticity of selected arrangements should have been verified beforehand
        foreach (ArrangementQuantity arrQuantity in newOrder.Arrangements)
        {
            total += (arrQuantity.Quantity * arrQuantity.Price);
        }

        if (screening.Hall.Has3DProjector)
        {
            total += 2.50m; // 3D fee
        }

        return total;
    }

    public decimal CalculatePrice(Screening screening, TicketType ticketType)
    {
        var price = ticketType switch
        {
            TicketType.Regular => CalculateRegularPrice(screening),
            TicketType.Child => CalculateChildPrice(screening),
            TicketType.Senior => CaculateSeniorPrice(screening),
            TicketType.Student => CalculateStudentPrice(screening),
            _ => throw new NotImplementedException($"Invalid TicketType {ticketType}"),
        };

        return price;
    }

    private static decimal CalculateRegularPrice(Screening screening) => screening.Movie.Duration > 120 ? 9.00m : 8.50m;

    private static decimal CalculateChildPrice(Screening screening)
    {
        var price = CalculateRegularPrice(screening);

        if (screening.StartTime.Hour <= 18 && screening.Movie.MinimumAge < 12 && screening.Movie.Language == "nl")
        {
            price -= 1.5m;
        }

        return price;
    }

    private static decimal CaculateSeniorPrice(Screening screening)
    {
        var price = CalculateRegularPrice(screening);

        if (screening.StartTime.DayOfWeek >= DayOfWeek.Monday && screening.StartTime.DayOfWeek <= DayOfWeek.Thursday && !screening.StartTime.Date.IsNationalHoliday())
        {
            price -= 1.5m;
        }

        return price;
    }

    private static decimal CalculateStudentPrice(Screening screening)
    {
        var price = CalculateRegularPrice(screening);

        if (screening.StartTime.DayOfWeek >= DayOfWeek.Monday && screening.StartTime.DayOfWeek <= DayOfWeek.Thursday)
        {
            price -= 1.5m;
        }

        return price;
    }
}