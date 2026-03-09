namespace ConnectPlay.TicketPlay.API.Extensions;

public static class DateTimeExtensions
{
    private static readonly List<DateOnly> Holidays = [
        new DateOnly(1, 1, 1),
        new DateOnly(1, 12, 25),
        new DateOnly(1, 12, 26),
        ];

    public static bool IsNationalHoliday(this DateTime date)
    {
        return Holidays.Any(holiday => holiday.Day == date.Day && holiday.Month == date.Month);
    }
}