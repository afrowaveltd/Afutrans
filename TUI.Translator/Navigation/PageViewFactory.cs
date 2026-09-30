using Afutrans.Core.ViewModels;
using TUI.Translator.Views.Pages;
using Terminal.Gui.ViewBase;

namespace TUI.Translator.Navigation;

/// <summary>
/// Maps a shared page view model to the Terminal.Gui view that renders it.
/// </summary>
/// <remarks>
/// This is the TUI counterpart of Avalonia's <c>ViewLocator</c>: the shell hands over whatever
/// <see cref="ViewModelBase"/> the navigation service produced and gets a view back.
/// </remarks>
public sealed class PageViewFactory
{
    /// <summary>Creates the view for <paramref name="viewModel"/>.</summary>
    public View Create(ViewModelBase viewModel) =>
        viewModel switch
        {
            MainViewModel main => new MainPageView(main),
            SetupViewModel setup => new SetupPageView(setup),
            TestViewModel test => new TestPageView(test),
            _ => throw new NotSupportedException($"No Terminal.Gui view is registered for '{viewModel.GetType().FullName}'."),
        };
}
