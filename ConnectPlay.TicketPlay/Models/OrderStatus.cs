using System.Text.Json.Serialization;

namespace ConnectPlay.TicketPlay.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus : byte // backed as byte, will be a number in database 
{
    Pending,
    Paid,
    Redeemed, // for when the tickets have been printed out
    Cancelled
}
