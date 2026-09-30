using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Afutrans.Core.ViewModels;
using Translator.Views;
using Translator.Views.Pages;

namespace Translator;

/// <summary>
/// Maps a shared view model to the Avalonia view that renders it.
/// </summary>
/// <remarks>
/// Replaces the template <c>ViewLocator</c> (which relied on naming conventions and reflection)
/// with an explicit registry: adding a page means adding one line here and one
/// <c>AddAfutransPage&lt;TPage&gt;()</c> call in <see cref="Program"/>.
/// </remarks>
public sealed class ViewLocator : IDataTemplate
{
    /// <inheritdoc />
    public Control? Build(object? param) =>
        param switch
        {
            MainViewModel main => new MainView { DataContext = main },
            SetupViewModel setup => new SetupView { DataContext = setup },
            TestViewModel test => new TestView { DataContext = test },
            _ => new TextBlock { Text = $"No Avalonia view registered for {param?.GetType().Name ?? "null"}." },
        };

    /// <inheritdoc />
    public bool Match(object? data) => data is ViewModelBase;
}
