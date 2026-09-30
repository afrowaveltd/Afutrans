using Afutrans.Core.ViewModels;

namespace Afutrans.Core.Navigation;

/// <summary>
/// Swaps the *content* of the application shell while the header and the footer stay untouched.
/// </summary>
/// <remarks>
/// Every front end (Avalonia GUI, Terminal.Gui TUI) uses this same service, so navigation
/// behaves identically in both. The shell observes <see cref="Navigated"/> and re-hosts
/// <see cref="CurrentViewModel"/> in its content area.
/// </remarks>
public interface INavigationService
{
    /// <summary>Gets the routes that can be navigated to, in display order.</summary>
    IReadOnlyList<NavigationRoute> Routes { get; }

    /// <summary>Gets the view model of the page that is currently shown, if any.</summary>
    ViewModelBase? CurrentViewModel { get; }

    /// <summary>Gets the route of the page that is currently shown, if any.</summary>
    NavigationRoute? CurrentRoute { get; }

    /// <summary>Raised after the content page changed.</summary>
    event EventHandler<NavigationEventArgs>? Navigated;

    /// <summary>Navigates to the route with <paramref name="routeKey"/>; re-navigating to the current route is a no-op.</summary>
    /// <returns><see langword="true"/> when a navigation happened.</returns>
    bool NavigateTo(string routeKey);

    /// <summary>Navigates to the route registered for <typeparamref name="TViewModel"/>.</summary>
    /// <returns><see langword="true"/> when a navigation happened.</returns>
    bool NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
}

/// <summary>Event data for <see cref="INavigationService.Navigated"/>.</summary>
public sealed class NavigationEventArgs(ViewModelBase currentViewModel, NavigationRoute route) : EventArgs
{
    /// <summary>Gets the view model that is now current.</summary>
    public ViewModelBase CurrentViewModel { get; } = currentViewModel;

    /// <summary>Gets the route that was navigated to.</summary>
    public NavigationRoute Route { get; } = route;
}
