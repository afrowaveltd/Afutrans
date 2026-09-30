using System.ComponentModel;
using Afutrans.Core.Localization;
using Afutrans.Core.ViewModels;
using TUI.Translator.Binding;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TUI.Translator.Views;

/// <summary>
/// The application shell: a **static** header (title + current page), a **static** footer and a
/// content area that is the only thing replaced when the user navigates.
/// </summary>
/// <remarks>
/// <para>Header, footer and the navigation shortcuts are built once in the constructor and bound
/// to the <see cref="ShellViewModel"/>; they never take part in the swap.</para>
/// <para>Navigation is triggered by the menu, by the F2/F3/F4 shortcuts and by the page itself —
/// all of them call <see cref="ShellViewModel.NavigateCommand"/>.</para>
/// </remarks>
public sealed class ShellWindow : Runnable
{
    private readonly ShellViewModel _shell;
    private readonly Func<ViewModelBase, View> _viewFactory;
    private readonly List<IDisposable> _bindings = [];

    private View? _currentPageView;

    /// <summary>Initializes the shell, its static chrome and the page that is shown first.</summary>
    /// <param name="shell">Shell view model (owns navigation and the static texts).</param>
    /// <param name="viewFactory">Maps a page view model to the view that renders it.</param>
    public ShellWindow(ShellViewModel shell, Func<ViewModelBase, View> viewFactory)
    {
        _shell = shell ?? throw new ArgumentNullException(nameof(shell));
        _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));

        Title = shell.AppTitle;

        // --- static header --------------------------------------------------
        MenuBar menuBar = new()
        {
            Menus = [BuildNavigationMenu(), BuildHelpMenu()],
        };

        HeaderTitle = new Label { X = 1, Y = 0, Width = Dim.Fill() };
        HeaderPage = new Label { X = 1, Y = 1, Width = Dim.Fill() };

        View header = new()
        {
            X = 0,
            Y = Pos.Bottom(menuBar),
            Width = Dim.Fill(),
            Height = 2,
        };

        header.Add(HeaderTitle, HeaderPage);
        Header = header;

        // --- static footer (text row + key hints) ---------------------------
        FooterText = new Label { X = 1, Y = 0, Width = Dim.Fill() };

        View footer = new()
        {
            X = 0,
            Y = Pos.AnchorEnd(2),
            Width = Dim.Fill(),
            Height = 1,
        };

        footer.Add(FooterText);
        Footer = footer;

        Status = new StatusBar();
        Status.Y = Pos.AnchorEnd(1);
        Status.Add(new Shortcut(Key.F2, "Main", () => _shell.NavigateCommand.Execute(StringKeys.NavMain)));
        Status.Add(new Shortcut(Key.F3, "Setup", () => _shell.NavigateCommand.Execute(StringKeys.NavSetup)));
        Status.Add(new Shortcut(Key.F4, "Test", () => _shell.NavigateCommand.Execute(StringKeys.NavTest)));
        Status.Add(new Shortcut(Key.Q.WithCtrl, "Quit", () => App?.RequestStop()));

        // --- content area (the only part that changes) ----------------------
        Content = new View
        {
            X = 0,
            Y = Pos.Bottom(header),
            Width = Dim.Fill(),
            Height = Dim.Fill(2),
        };

        Add(menuBar, header, Content, footer, Status);

        // The static chrome only ever binds to the shell view model.
        _bindings.Add(ViewBinder.OneWay<string>(shell, nameof(ShellViewModel.AppTitle), value => HeaderTitle.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(shell, nameof(ShellViewModel.CurrentPageTitle), value => HeaderPage.Text = $"» {value}"));
        _bindings.Add(ViewBinder.OneWay<string>(shell, nameof(ShellViewModel.FooterText), value => FooterText.Text = value));

        shell.PropertyChanged += OnShellPropertyChanged;

        ShowPage(shell.CurrentViewModel);
    }

    /// <summary>Gets the static header title label (bound to the shell, never swapped).</summary>
    public Label HeaderTitle { get; }

    /// <summary>Gets the static header label showing the current page title.</summary>
    public Label HeaderPage { get; }

    /// <summary>Gets the header container.</summary>
    public View Header { get; }

    /// <summary>Gets the static footer text label.</summary>
    public Label FooterText { get; }

    /// <summary>Gets the footer container.</summary>
    public View Footer { get; }

    /// <summary>Gets the status bar with the navigation key hints.</summary>
    public StatusBar Status { get; }

    /// <summary>Gets the content host; its sub view is the current page view.</summary>
    public View Content { get; }

    /// <summary>Gets the view of the page that is currently hosted.</summary>
    public View? CurrentPageView => _currentPageView;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            _shell.PropertyChanged -= OnShellPropertyChanged;

            foreach(var binding in _bindings)
            {
                binding.Dispose();
            }

            _bindings.Clear();
        }

        base.Dispose(disposing);
    }

    private void OnShellPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName is null or nameof(ShellViewModel.CurrentViewModel))
        {
            ShowPage(_shell.CurrentViewModel);
        }
    }

    private void ShowPage(ViewModelBase? viewModel)
    {
        if(_currentPageView is not null)
        {
            Content.Remove(_currentPageView);
            _currentPageView.Dispose();
            _currentPageView = null;
        }

        if(viewModel is null)
        {
            return;
        }

        _currentPageView = _viewFactory(viewModel);
        Content.Add(_currentPageView);
        Content.SetNeedsLayout();
        Content.SetNeedsDraw();
    }

    private MenuBarItem BuildNavigationMenu() =>
        new("_Navigate",
        [
            new MenuItem("_Main", "F2", () => _shell.NavigateCommand.Execute(StringKeys.NavMain)),
            new MenuItem("_Setup", "F3", () => _shell.NavigateCommand.Execute(StringKeys.NavSetup)),
            new MenuItem("_Test", "F4", () => _shell.NavigateCommand.Execute(StringKeys.NavTest)),
        ]);

    private MenuBarItem BuildHelpMenu() =>
        new("_Help",
        [
            new MenuItem("_About", "", () => MessageBox.Query(App!, "About", "Afutrans Translator\nShared core demo: MVVM Toolkit · DI · Navigation · Localization", "OK")),
        ]);
}
