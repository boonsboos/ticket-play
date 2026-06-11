using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConnectPlay.TicketPlay.UI.Native.Services;

public class NotificationRouter : IHostedService
{
    private readonly INotificationService _notificationService;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<NotificationRouter> _logger;

    public NotificationRouter(INotificationService notificationService, NavigationManager navigationManager, ILogger<NotificationRouter> logger)
    {
        this._notificationService = notificationService;
        this._navigationManager = navigationManager;
        this._logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this._notificationService.NotificationReceived += HandleNotification;
    }

    private void HandleNotification(object? sender, NotificationEventArgs args)
    {
        var target = args.Notification.Path;

        this._logger.LogInformation("Notification target is {Path}", target);

        this._navigationManager.NavigateTo(target, replace: true);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        this._notificationService.NotificationReceived -= HandleNotification;
    }
}
