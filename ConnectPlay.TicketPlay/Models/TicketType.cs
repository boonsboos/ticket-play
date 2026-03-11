using System.Text.Json.Serialization;

namespace ConnectPlay.TicketPlay.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TicketType : byte
{
    Regular,
    Child,
    Student,
    Senior // 65+
}
