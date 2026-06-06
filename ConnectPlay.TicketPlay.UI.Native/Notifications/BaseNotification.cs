namespace ConnectPlay.TicketPlay.UI.Native.Notifications;

public class BaseNotification
{
    public required string Title { get; init; }
    public required string Message { get; init; }
    public string Path { get; init; } = "/";
    public DateTimeOffset? NotifyAt { get; init; } = null;
}
