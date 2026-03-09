namespace ConnectPlay.TicketPlay.Models;

public enum OrderStatus : byte // backed as byte, will be a number in database 
{
    Pending,
    Paid,
    Redeemed, // for when the tickets have been printed out
    Canceled
}
