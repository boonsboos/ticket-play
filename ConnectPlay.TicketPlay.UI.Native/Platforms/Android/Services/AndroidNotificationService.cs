using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Events;
using ConnectPlay.TicketPlay.UI.Native.Notifications;
using ConnectPlay.TicketPlay.UI.Native.Resources;
using System.Diagnostics.CodeAnalysis;

namespace ConnectPlay.TicketPlay.UI.Native.Platforms.Android.Services;

// Adapted from the MAUI Local Notifications tutorial
// Note: this implementation does not recreate the notification alarms/broadcasts when the device is restarted.
internal class AndroidNotificationService : INotificationService
{
    const string channelId = "ticketplay";
    const string channelName = "Ticket & Play Notifications";
    private readonly string channelDescription = AppResources.Notification_ChannelDescription;

    public const string TitleKey = "title";
    public const string MessageKey = "message";
    public const string PathKey = "path";

    private bool channelInitialized = false;
    private int messageId = 0;
    private int pendingIntentId = 0;

    private readonly NotificationManagerCompat compatManager;

    public event EventHandler<NotificationEventArgs>? NotificationReceived;

    public static AndroidNotificationService Instance { get; private set; } = new();

    public AndroidNotificationService()
    {
        CreateNotificationChannel();
        compatManager = NotificationManagerCompat.From(Platform.AppContext)!;
    }

    public void SendNotification(BaseNotification baseNotification)
    {
        if (!channelInitialized)
        {
            CreateNotificationChannel();
        }

        // We're not handling a timed notification, just show the notification
        if (baseNotification.NotifyAt is null)
        {
            Show(baseNotification);
            return;
        }

        Intent intent = new(Platform.AppContext, typeof(TimedNotificationHandler));
        intent.PutExtra(TitleKey, baseNotification.Title);
        intent.PutExtra(MessageKey, baseNotification.Message);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.CancelCurrent;

        PendingIntent pendingIntent = PendingIntent.GetBroadcast(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags)!;

        long triggerTime = GetNotifyTime((DateTimeOffset)baseNotification.NotifyAt);
        AlarmManager alarmManager = (AlarmManager) Platform.AppContext.GetSystemService(Context.AlarmService)!;
        alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
    }

    public void ReceiveNotification(BaseNotification baseNotification)
    {
        var args = new NotificationEventArgs()
        {
            Notification = new BaseNotification()
            {
                Title = baseNotification.Title,
                Message = baseNotification.Message,
                Path = baseNotification.Path
            }
        };
        NotificationReceived?.Invoke(null, args);
    }

    public void Show(BaseNotification baseNotification)
    {
        Intent intent = new(Platform.AppContext, typeof(MainActivity));
        intent.PutExtra(TitleKey, baseNotification.Title);
        intent.PutExtra(MessageKey, baseNotification.Message);
        intent.PutExtra(PathKey, baseNotification.Path);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.UpdateCurrent;

        PendingIntent pendingIntent = PendingIntent.GetActivity(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags)!;
        NotificationCompat.Builder builder = new NotificationCompat.Builder(Platform.AppContext, channelId)
            .SetContentIntent(pendingIntent)!
            .SetContentTitle(baseNotification.Title)!
            .SetContentText(baseNotification.Message)!;

        Notification notification = builder.Build()!;
        compatManager.Notify(messageId++, notification);
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Version guard already present in method")]
    private void CreateNotificationChannel()
    {
        // Create the notification channel, but only on API 26+.
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channelNameJava = new Java.Lang.String(channelName);
            var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            // Register the channel
            NotificationManager manager = (NotificationManager)Platform.AppContext.GetSystemService(Context.NotificationService)!;
            manager.CreateNotificationChannel(channel);
            channelInitialized = true;
        }
    }

    private static long GetNotifyTime(DateTimeOffset notifyTime)
    {
        return (long) (notifyTime.UtcDateTime - DateTime.UnixEpoch).TotalMilliseconds;
    }
}
