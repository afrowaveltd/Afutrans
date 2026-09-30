using System.Globalization;
using Afutrans.Core.DependencyInjection;
using Afutrans.Core.Localization;
using Afutrans.Core.ViewModels;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Translator.Views;

namespace Translator;

/// <summary>
/// Avalonia application. Builds the dependency injection container (the same shared
/// <c>AddAfutransCore()</c> bootstrap the TUI uses) and hands the shell view model to the window.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _services;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _services = CreateServices();

            desktop.MainWindow = new MainWindow
            {
                // The window is the static chrome; the shell view model drives navigation.
                DataContext = _services.GetRequiredService<ShellViewModel>(),
            };

            desktop.ShutdownRequested += (_, _) => _services?.Dispose();
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>Builds the DI container: shared core services, then the three pages of the shell.</summary>
    public static ServiceProvider CreateServices(CultureInfo? culture = null)
    {
        var services = new ServiceCollection();

        services.AddAfutransCore(culture);
        services.AddAfutransPage<MainViewModel>(StringKeys.NavMain);
        services.AddAfutransPage<SetupViewModel>(StringKeys.NavSetup);
        services.AddAfutransPage<TestViewModel>(StringKeys.NavTest);

        return services.BuildServiceProvider();
    }
}
