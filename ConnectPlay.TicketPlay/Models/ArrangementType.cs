using System.Text.Json.Serialization;

namespace ConnectPlay.TicketPlay.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ArrangementType : byte
{
    Snack,
    Drink,
    Special
}