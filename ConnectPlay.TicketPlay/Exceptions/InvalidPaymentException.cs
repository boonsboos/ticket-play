namespace ConnectPlay.TicketPlay.Exceptions;

public class InvalidPaymentException : Exception
{
    public InvalidPaymentException(string? message) : base(message)
    {
    }

    public InvalidPaymentException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}