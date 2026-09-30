using Afutrans.Core.Localization;
using Afutrans.Core.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Afutrans.Core.ViewModels;

/// <summary>
/// View model of the application shell: the **static** header (title + navigation) and the
/// **static** footer are bound to this view model, while only the content area is swapped when
/// the user navigates.
/// </summary>
public sealed partial class ShellViewModel : ViewModelBase, IDisposable
{
    private readonly INavigationService _navigation;
    private readonly ILocalizer _localizer;

    /// <summary>Initializes a new shell view model, rendering the current page and building the nav strip.</summary>
    /// <param name="navigation">Navigation service that owns the route table.</param>
    /// <param name="localizer">Localizer used for the header, the nav strip and the footer.</param>
    /// <param name="startRouteKey">Route to show first; defaults to the first registered route.</param>
    public ShellViewModel(INavigationService navigation, ILocalizer localizer, string? startRouteKey = null)
        : base(localizer)
    {
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        NavigationItems = [.. _navigation.Routes.Select(route => new NavigationItemViewModel(_localizer, route.Key))];

        _navigation.Navigated += OnNavigated;
        _localizer.CultureChanged += OnCultureChanged;

        var firstRouteKey = startRouteKey ?? _navigation.Routes.FirstOrDefault()?.Key
            ?? throw new InvalidOperationException("No navigation route is registered; call AddAfutransPage() during service registration.");

        NavigateCommand = new RelayCommand<string>(Navigate);

        _navigation.NavigateTo(firstRouteKey);
    }

    /// <summary>Gets the localized application title shown in the static header.</summary>
    public string AppTitle => _localizer[StringKeys.AppTitle];

    /// <summary>Gets the localized footer text (static chrome, never replaced by navigation).</summary>
    public string FooterText => _localizer[StringKeys.AppFooter];

    /// <summary>Gets the nav strip entries.</summary>
    public IReadOnlyList<NavigationItemViewModel> NavigationItems { get; }

    /// <summary>Gets the view model currently hosted in the content area.</summary>
    public ViewModelBase? CurrentViewModel => _navigation.CurrentViewModel;

    /// <summary>Gets the localized title of the current page (used by the TUI header).</summary>
    public string CurrentPageTitle => (_navigation.CurrentViewModel as PageViewModelBase)?.Title ?? string.Empty;

    /// <summary>Gets the route key of the current page.</summary>
    public string? CurrentRouteKey => _navigation.CurrentRoute?.Key;

    /// <summary>Navigates to the route with the given key (bound to the nav buttons in both front ends).</summary>
    public IRelayCommand<string> NavigateCommand { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        _navigation.Navigated -= OnNavigated;
        _localizer.CultureChanged -= OnCultureChanged;
    }

    private void Navigate(string? routeKey)
    {
        if(!string.IsNullOrWhiteSpace(routeKey))
        {
            _navigation.NavigateTo(routeKey);
        }
    }

    private void OnNavigated(object? sender, NavigationEventArgs e)
    {
        foreach(var item in NavigationItems)
        {
            item.IsCurrent = string.Equals(item.RouteKey, e.Route.Key, StringComparison.Ordinal);
        }

        OnPropertyChanged(nameof(CurrentViewModel));
        OnPropertyChanged(nameof(CurrentPageTitle));
        OnPropertyChanged(nameof(CurrentRouteKey));
    }

    private void OnCultureChanged(object? sender, CultureChangedEventArgs e)
    {
        foreach(var item in NavigationItems)
        {
            item.RefreshLocalization();
        }

        OnPropertyChanged(nameof(AppTitle));
        OnPropertyChanged(nameof(FooterText));
        OnPropertyChanged(nameof(CurrentPageTitle));

        // Let the hosted page refresh its own localized strings.
        (_navigation.CurrentViewModel as PageViewModelBase)?.OnNavigatedTo();
    }
}
