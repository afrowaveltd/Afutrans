namespace Afutrans.Core.Navigation;

/// <summary>
/// A navigable destination: a stable key (also used as the localized title key) and the
/// view model type that renders it.
/// </summary>
/// <param name="Key">Stable route key, e.g. <see cref="Localization.StringKeys.NavMain"/>.</param>
/// <param name="ViewModelType">View model type created when the route is navigated to.</param>
public sealed record NavigationRoute(string Key, Type ViewModelType)
{
    /// <summary>Gets the key used to look up the localized title of this route.</summary>
    public string TitleKey => Key;
}
