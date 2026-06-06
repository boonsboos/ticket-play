using ConnectPlay.TicketPlay.UI.Native.Notifications;

namespace ConnectPlay.TicketPlay.UI.Native.Events;

public class NotificationEventArgs : EventArgs
{
    public required BaseNotification Notification { get; init; }
}
