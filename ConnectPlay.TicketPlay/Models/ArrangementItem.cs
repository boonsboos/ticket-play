using System.Text.Json.Serialization;

namespace ConnectPlay.TicketPlay.Models;

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ArrangementItem : byte
{
    SmallPopcorn = 1 << 0,
    MediumPopcorn = 1 << 1,
    LargePopcorn = 1 << 2,

    Cola = 1 << 3,
    ColaZero = 1 << 4,
    Fanta = 1 << 5,
    Sprite = 1 << 6,
    IceTea = 1 << 7,
}
