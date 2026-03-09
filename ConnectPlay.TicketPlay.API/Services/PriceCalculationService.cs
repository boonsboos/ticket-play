using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.API.Extensions;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Services;

public class PriceCalculationService : IPriceCalculationService
{
    public float CalculatePrices(Screening screening, IEnumerable<TicketType> ticketTypes)
    {
        var total = 0f;
        foreach (TicketType ticketType in ticketTypes)
        {
            CalculatePrice(screening, ticketType);
        }
        return total;
    }

    public float CalculatePrice(Screening screening, TicketType ticketType)
    {
        var price = ticketType switch
        {
            TicketType.Regular => CalculateRegularPrice(screening),
            TicketType.Child => CalculateChildPrice(screening),
            TicketType.Senior => CaculateSeniorPrice(screening),
            TicketType.Student => CalculateStudentPrice(screening),
            _ => throw new NotImplementedException($"Invalid TicketType {ticketType}"),
        };

        if (screening.Hall.Has3DProjector)
        {
            price += 2.50f; // 3D fee
        }

        return price;
    }

    private static float CalculateRegularPrice(Screening screening) => screening.Movie.Duration > 120 ? 9.00f : 8.50f;

    private static float CalculateChildPrice(Screening screening)
    {
        var price = CalculateRegularPrice(screening);

        if (screening.StartTime.Hour <= 18 && screening.Movie.MinimumAge < 12 && screening.Movie.Language == "nl")
        {
            price -= 1.5f;
        }

        return price;
    }

    private static float CaculateSeniorPrice(Screening screening)
    {
        var price = CalculateRegularPrice(screening);

        if (screening.StartTime.DayOfWeek >= DayOfWeek.Monday && screening.StartTime.DayOfWeek <= DayOfWeek.Thursday && !screening.StartTime.Date.IsNationalHoliday())
        {
            price -= 1.5f;
        }

        return price;
    }

    private static float CalculateStudentPrice(Screening screening)
    {
        var price = CalculateRegularPrice(screening);

        if (screening.StartTime.DayOfWeek >= DayOfWeek.Monday && screening.StartTime.DayOfWeek <= DayOfWeek.Thursday)
        {
            price -= 1.5f;
        }

        return price;
    }
}