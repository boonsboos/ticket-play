using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Notifications;
using ConnectPlay.TicketPlay.UI.Native.Resources;
using Microsoft.Extensions.Hosting;

namespace ConnectPlay.TicketPlay.UI.Native.Services;

public class GeolocationService : IHostedService
{
    private static readonly Location CinemaLocation = new Location(51.5840246, 4.797961);

    private readonly INotificationService _notificationService;
    private readonly IGeolocation _geolocation;

    public GeolocationService(INotificationService notificationService, IGeolocation geolocation)
    {
        this._notificationService = notificationService;
        this._geolocation = geolocation;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this._geolocation.LocationChanged += OnLocationChange;

        var request = new GeolocationListeningRequest(GeolocationAccuracy.Medium);

        // if the exception is thrown that should be fine
        var result = this._geolocation.StartListeningForegroundAsync(request);
    }

    private void OnLocationChange(object? sender, GeolocationLocationChangedEventArgs args)
    {
        var difference = Location.CalculateDistance(args.Location, CinemaLocation, DistanceUnits.Kilometers);

        // if we're 1km away, it's within walking distance
        if (difference <= 1)
        {
            _notificationService.SendNotification(new BaseNotification
            {
                Title = AppResources.Notification_CloseTitle,
                Message = AppResources.Notification_Close,
                Path = "/"
            });
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        this._geolocation.LocationChanged -= OnLocationChange;
        this._geolocation.StopListeningForeground();
    }
}
