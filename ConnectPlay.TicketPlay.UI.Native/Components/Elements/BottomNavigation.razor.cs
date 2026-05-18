using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Elements;

public partial class BottomNavigation : ComponentBase
{
    private readonly NavigationManager _navigationManager;

    [Parameter]
    public CurrentView Current { get => _current; set => _current = value; }

    public BottomNavigation(NavigationManager navigationManager)
    {
        this._navigationManager = navigationManager;
    }

    private CurrentView _current = CurrentView.Home;

    private void NavigateYou()
    {
        if (_current == CurrentView.You) return;

        _current = CurrentView.You;

        _navigationManager.NavigateTo("/you");
    }

    private void NavigateHome()
    {
        if (_current == CurrentView.Home) return;

        _current = CurrentView.Home;

        _navigationManager.NavigateTo("/");
    }

    private void NavigateTickets()
    {
        if (_current == CurrentView.Tickets) return;

        _current = CurrentView.Tickets;

        _navigationManager.NavigateTo("/tickets");
    }

    public void Unset()
    {
        this._current = CurrentView.Unset;
    }

    public enum CurrentView
    {
        Unset,
        You,
        Home,
        Tickets
    }
}
