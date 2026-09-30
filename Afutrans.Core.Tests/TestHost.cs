using Afutrans.Core.DependencyInjection;
using Afutrans.Core.Localization;
using Afutrans.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Afutrans.Core.Tests;

/// <summary>Shared factory that builds the same DI container the two front ends use.</summary>
internal static class TestHost
{
    public static ServiceProvider Build(params string[] startOrder)
    {
        var services = new ServiceCollection();
        services.AddAfutransCore();

        var pages = startOrder.Length == 0
            ? new[] { StringKeys.NavMain, StringKeys.NavSetup, StringKeys.NavTest }
            : startOrder;

        foreach(var routeKey in pages)
        {
            switch(routeKey)
            {
                case StringKeys.NavMain:
                    services.AddAfutransPage<MainViewModel>(StringKeys.NavMain);

                    break;
                case StringKeys.NavSetup:
                    services.AddAfutransPage<SetupViewModel>(StringKeys.NavSetup);

                    break;
                case StringKeys.NavTest:
                    services.AddAfutransPage<TestViewModel>(StringKeys.NavTest);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(startOrder), routeKey, "Unknown test route.");
            }
        }

        return services.BuildServiceProvider();
    }

    public static ShellViewModel CreateShell(out ServiceProvider provider, string? startRoute = null)
    {
        provider = Build();

        var localizer = provider.GetRequiredService<ILocalizer>();
        var navigation = provider.GetRequiredService<Afutrans.Core.Navigation.INavigationService>();

        return new ShellViewModel(navigation, localizer, startRoute);
    }
}
