using Android.Content;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Notifications;
using ConnectPlay.TicketPlay.UI.Native.Platforms.Android.Services;

namespace ConnectPlay.TicketPlay.UI.Native.Platforms.Android;

[BroadcastReceiver(Enabled = true, Label = "Timed notification broadcast receiver")]
public class TimedNotificationHandler : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent?.Extras is null) {
            return;
        }
        
        // these are all not null if they come from us
        string title = intent.GetStringExtra(AndroidNotificationService.TitleKey)!;
        string message = intent.GetStringExtra(AndroidNotificationService.MessageKey)!;
        string path = intent.GetStringExtra(AndroidNotificationService.PathKey)!;

        AndroidNotificationService.Instance.Show(new BaseNotification
        {
            Message = message!,
            Title = title!,
            Path = path ?? "/"
        });
    }
}
