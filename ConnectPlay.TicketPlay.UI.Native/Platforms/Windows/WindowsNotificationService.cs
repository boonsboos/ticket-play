using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Events;
using ConnectPlay.TicketPlay.UI.Native.Notifications;
using Microsoft.Extensions.Logging;

namespace ConnectPlay.TicketPlay.UI.Native.Platforms.Windows;

public class WindowsNotificationService : INotificationService
{
    private readonly ILogger<WindowsNotificationService> _logger;

    public event EventHandler<NotificationEventArgs>? NotificationReceived;

    public WindowsNotificationService(ILogger<WindowsNotificationService> logger)
    {
        this._logger = logger;
    }

    public void ReceiveNotification(BaseNotification baseNotification)
    {
        _logger.LogInformation("Recieved a notification");
    }

    public void SendNotification(BaseNotification baseNotification)
    {
        _logger.LogInformation("Sending a notification");
        Show(baseNotification);
    }

    public void Show(BaseNotification baseNotification)
    {
        _logger.LogInformation("Showing a notification");

        Task.Run(() =>
        {
            Task.Delay(2000);

            NotificationReceived?.Invoke(this, new NotificationEventArgs { Notification = baseNotification });
        });
    }
}
