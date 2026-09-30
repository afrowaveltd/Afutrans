namespace Afutrans.Core.Navigation;

/// <summary>
/// Mutable route table filled by <c>AddAfutransPage&lt;TPage&gt;(...)</c> during service registration
/// and read by <see cref="NavigationService"/>.
/// </summary>
public sealed class NavigationRouteTable
{
    private readonly List<NavigationRoute> _routes = [];

    /// <summary>Gets the registered routes in registration (display) order.</summary>
    public IReadOnlyList<NavigationRoute> Routes => _routes;

    /// <summary>Adds a route; throws when the key is already registered.</summary>
    public NavigationRouteTable Add(NavigationRoute route)
    {
        ArgumentNullException.ThrowIfNull(route);

        if(_routes.Any(r => string.Equals(r.Key, route.Key, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException($"A route with the key '{route.Key}' is already registered.");
        }

        _routes.Add(route);

        return this;
    }
}
