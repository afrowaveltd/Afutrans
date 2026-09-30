using Afutrans.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Afutrans.Core.Navigation;

/// <summary>
/// Default <see cref="INavigationService"/>: resolves a fresh page view model from a new DI scope
/// on every navigation and disposes the previous scope (which disposes the previous page).
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private IServiceScope? _currentScope;

    /// <summary>Initializes a new navigation service.</summary>
    public NavigationService(IServiceProvider serviceProvider, NavigationRouteTable routeTable)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(routeTable);

        _serviceProvider = serviceProvider;
        Routes = routeTable.Routes;
    }

    /// <inheritdoc />
    public IReadOnlyList<NavigationRoute> Routes { get; }

    /// <inheritdoc />
    public ViewModelBase? CurrentViewModel { get; private set; }

    /// <inheritdoc />
    public NavigationRoute? CurrentRoute { get; private set; }

    /// <inheritdoc />
    public event EventHandler<NavigationEventArgs>? Navigated;

    /// <inheritdoc />
    public bool NavigateTo(string routeKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(routeKey);

        var route = Routes.FirstOrDefault(r => string.Equals(r.Key, routeKey, StringComparison.Ordinal))
                    ?? throw new InvalidOperationException($"No route registered for key '{routeKey}'.");

        return Navigate(route);
    }

    /// <inheritdoc />
    public bool NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var route = Routes.FirstOrDefault(r => r.ViewModelType == typeof(TViewModel))
                    ?? throw new InvalidOperationException($"No route registered for view model '{typeof(TViewModel).FullName}'.");

        return Navigate(route);
    }

    private bool Navigate(NavigationRoute route)
    {
        // Re-entering the current page must not throw away its state (for example a half filled form).
        if(CurrentRoute is not null && string.Equals(CurrentRoute.Key, route.Key, StringComparison.Ordinal))
        {
            return false;
        }

        var previous = CurrentViewModel;

        IServiceScope scope = _serviceProvider.CreateScope();
        ViewModelBase viewModel;

        try
        {
            viewModel = (ViewModelBase)scope.ServiceProvider.GetRequiredService(route.ViewModelType);
        }
        catch(Exception exception)
        {
            scope.Dispose();

            throw new InvalidOperationException($"Unable to create the view model '{route.ViewModelType.FullName}'.", exception);
        }

        // Tell the outgoing page first, then swap the scope (which disposes the old page).
        if(previous is INavigableViewModel outgoing && !ReferenceEquals(previous, viewModel))
        {
            outgoing.OnNavigatedFrom();
        }

        _currentScope?.Dispose();
        _currentScope = scope;
        CurrentViewModel = viewModel;
        CurrentRoute = route;

        if(viewModel is INavigableViewModel incoming)
        {
            incoming.OnNavigatedTo();
        }

        Navigated?.Invoke(this, new NavigationEventArgs(viewModel, route));

        return true;
    }
}
