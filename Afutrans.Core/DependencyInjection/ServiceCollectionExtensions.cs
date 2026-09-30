using System.Globalization;
using Afutrans.Core.Localization;
using Afutrans.Core.Navigation;
using Afutrans.Core.Services;
using Afutrans.Core.Validation;
using Afutrans.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Afutrans.Core.DependencyInjection;

/// <summary>
/// Registers the shared Afutrans services, view models and navigation routes.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the shared core (localization, navigation, practice services, shell view model).
    /// Call this first, then register the pages with <see cref="AddAfutransPage{TPage}"/>.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <param name="culture">Initial UI culture; English is the default.</param>
    public static IServiceCollection AddAfutransCore(this IServiceCollection services, CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var localizer = Localizer.CreateDefault(culture);

        // Validation attributes cannot use constructor injection, so they read the localizer
        // from this ambient slot — which keeps a single source of truth for the UI language.
        ValidationLocalizer.Current = localizer;

        services.AddSingleton<ILocalizer>(localizer);
        services.AddSingleton<IHelloService, HelloService>();

        var routes = new NavigationRouteTable();
        services.AddSingleton(routes);
        services.AddSingleton<INavigationService>(provider => new NavigationService(provider, routes));

        // The shell is the only long lived view model; pages are transient and scoped per navigation.
        services.AddSingleton<ShellViewModel>();

        return services;
    }

    /// <summary>Registers a page view model and adds it to the navigation route table.</summary>
    /// <typeparam name="TPage">The page view model type (must derive from <see cref="PageViewModelBase"/>).</typeparam>
    /// <param name="services">The service collection to extend.</param>
    /// <param name="routeKey">Stable route key; also the localization key of the page title.</param>
    public static IServiceCollection AddAfutransPage<TPage>(this IServiceCollection services, string routeKey)
        where TPage : PageViewModelBase
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(routeKey);

        var table = FindRouteTable(services);

        services.AddTransient<TPage>();
        table.Add(new NavigationRoute(routeKey, typeof(TPage)));

        return services;
    }

    /// <summary>Gets all registered routes (useful for tests and for front-end bootstrapping).</summary>
    public static IReadOnlyList<NavigationRoute> GetAfutransRoutes(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return FindRouteTable(services).Routes;
    }

    private static NavigationRouteTable FindRouteTable(IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(service => service.ServiceType == typeof(NavigationRouteTable));

        return descriptor?.ImplementationInstance as NavigationRouteTable
               ?? throw new InvalidOperationException($"Call {nameof(AddAfutransCore)}() before registering pages.");
    }
}
