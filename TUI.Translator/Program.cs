// -----------------------------------------------------------------------------
//  Afutrans translator — Terminal.Gui v2 front end.
//
//  The interesting wiring lives in Afutrans.Core (a shared class library that the
//  Avalonia GUI uses too):
//    * view models + validation + localization  -> Afutrans.Core.ViewModels
//    * navigation route table + service         -> Afutrans.Core.Navigation
//    * practice service (IHelloService)         -> Afutrans.Core.Services
//
//  This project only renders: ShellWindow.cs keeps a static header and footer and
//  swaps the content page; Views/Pages/*.cs bind to the shared view models.
//
//  Run:  dotnet run                 (English)
//        dotnet run -- --culture cs (Czech, to see the localizer switch)
//  Quit: Ctrl+Q / the File menu / the status bar.
//
//  🤖 AI assistants / agents: READ ./AGENTS.md FIRST — Terminal.Gui v2 is a
//     complete rewrite and pre-2025 examples will not compile.
// -----------------------------------------------------------------------------

using Afutrans.Core.DependencyInjection;
using Afutrans.Core.Localization;
using Afutrans.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui.App;           // Application, IApplication, MessageBox
using Terminal.Gui.Configuration;
using TUI.Translator.Navigation;
using TUI.Translator.Views;

new TuiConfigurationBuilder().ApplyToStaticFacades();

// English is the default language; `--culture cs` shows the shared localizer at work.
var culture = args
    .Select((argument, index) => (argument, index))
    .Where(item => item.argument == "--culture" && item.index + 1 < args.Length)
    .Select(item => new System.Globalization.CultureInfo(args[item.index + 1]))
    .FirstOrDefault();

// --- Dependency injection -----------------------------------------------------
var services = new ServiceCollection();

// Shared core: localizer, IHelloService, navigation service, shell view model.
services.AddAfutransCore(culture);

// The three pages of the shell; the route key doubles as the localization key of the title.
services.AddAfutransPage<MainViewModel>(StringKeys.NavMain);
services.AddAfutransPage<SetupViewModel>(StringKeys.NavSetup);
services.AddAfutransPage<TestViewModel>(StringKeys.NavTest);

using var provider = services.BuildServiceProvider();

// The shell is the only long lived view model; pages are created per navigation by the service.
using var shell = provider.GetRequiredService<ShellViewModel>();

var pageViews = new PageViewFactory();

// Instance lifecycle — NOT static Init/Run/Shutdown:  Create() -> Run(view) -> Dispose().
using var app = Application.Create();
using var window = new ShellWindow(shell, pageViews.Create);
app.Init().Run(window);
