using Afutrans.Core.Localization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Afutrans.Core.ViewModels;

/// <summary>
/// One entry of the navigation strip rendered by the application shell.
/// </summary>
public partial class NavigationItemViewModel : ViewModelBase
{
    /// <summary>Initializes a new navigation item.</summary>
    public NavigationItemViewModel(ILocalizer localizer, string routeKey)
        : base(localizer)
    {
        RouteKey = routeKey ?? throw new ArgumentNullException(nameof(routeKey));
    }

    /// <summary>Gets the route key this item navigates to.</summary>
    public string RouteKey { get; }

    /// <summary>Gets the localized caption of the item.</summary>
    public string Title => L(RouteKey);

    /// <summary>Gets or sets a value indicating whether this item is the page currently shown.</summary>
    [ObservableProperty]
    public partial bool IsCurrent { get; set; }

    /// <summary>Re-reads the localized caption after the language changed.</summary>
    public void RefreshLocalization() => OnPropertyChanged(nameof(Title));
}
