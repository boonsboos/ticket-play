using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Notifications;
using ConnectPlay.TicketPlay.UI.Native.Platforms.Android.Services;

namespace ConnectPlay.TicketPlay.UI.Native.Platforms.Android;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
    LaunchMode = LaunchMode.SingleTop
)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        CreateNotificationFromIntent(base.Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);

        CreateNotificationFromIntent(intent);
    }

    private void CreateNotificationFromIntent(Intent? intent)
    {
        if (intent?.Extras is null)
        {
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
