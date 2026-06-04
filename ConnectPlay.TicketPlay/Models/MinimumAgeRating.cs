using System.Text.Json.Serialization;

namespace ConnectPlay.TicketPlay.Models;

[JsonConverter(typeof(JsonStringEnumConverter<MinimumAgeRating>))]
public enum MinimumAgeRating : byte
{
    AL = 0,
    Age6 = 6,
    Age9 = 9,
    Age12 = 12,
    Age14 = 14,
    Age16 = 16,
    Age18 = 18
}
