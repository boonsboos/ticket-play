using ConnectPlay.TicketPlay.UI.Native.Events;
using ConnectPlay.TicketPlay.UI.Native.Notifications;

namespace ConnectPlay.TicketPlay.UI.Native.Abstract;

public interface INotificationService
{
    event EventHandler<NotificationEventArgs>? NotificationReceived;
    void SendNotification(BaseNotification baseNotification);
    void Show(BaseNotification baseNotification);
    void ReceiveNotification(BaseNotification baseNotification);
}
