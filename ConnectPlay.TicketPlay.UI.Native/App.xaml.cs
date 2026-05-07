namespace ConnectPlay.TicketPlay.UI.Native;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage()) { Title = "ConnectPlay.TicketPlay.UI.Native" };
    }
}
