// Headless tests for the TUI views — no real terminal, no InputInjector needed for *logic*.
//
// The point of these tests is the binding: they prove that the Terminal.Gui views talk to the
// very same shared view models as the Avalonia front end (Afutrans.Core), that navigation only
// swaps the content area, and that the registration form gates its submit button.

using Afutrans.Core.DependencyInjection;
using Afutrans.Core.Localization;
using Afutrans.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using TUI.Translator.Navigation;
using TUI.Translator.Views;
using TUI.Translator.Views.Pages;
using Terminal.Gui.Input;     // Command
using Terminal.Gui.ViewBase;  // View
using Terminal.Gui.Views;     // Label, Button
using Xunit;

[assembly: Xunit.v3.Parallelization(Mode = Xunit.Sdk.ParallelMode.None)]

public class ShellWindowTests
{
    private static (ServiceProvider Provider, ShellViewModel Shell) CreateShell()
    {
        var services = new ServiceCollection();
        services.AddAfutransCore();
        services.AddAfutransPage<MainViewModel>(StringKeys.NavMain);
        services.AddAfutransPage<SetupViewModel>(StringKeys.NavSetup);
        services.AddAfutransPage<TestViewModel>(StringKeys.NavTest);

        var provider = services.BuildServiceProvider();

        return (provider, provider.GetRequiredService<ShellViewModel>());
    }

    [Fact]
    public void Shell_starts_on_the_main_page_and_keeps_a_static_header_and_footer()
    {
        var (provider, shell) = CreateShell();
        using var window = new ShellWindow(shell, new PageViewFactory().Create);

        Assert.IsType<MainPageView>(window.CurrentPageView);
        Assert.Equal("Afutrans Translator", window.HeaderTitle.Text);
        Assert.Contains("Main", window.HeaderPage.Text);
        Assert.Equal("Shared core demo: MVVM Toolkit · Dependency Injection · Navigation · Localization", window.FooterText.Text);

        provider.Dispose();
    }

    [Fact]
    public void Navigating_swaps_only_the_content_view_and_leaves_the_chrome_untouched()
    {
        var (provider, shell) = CreateShell();
        using var window = new ShellWindow(shell, new PageViewFactory().Create);

        var headerBefore = window.HeaderTitle.Text;
        var footerBefore = window.FooterText.Text;
        var chromeCountBefore = window.SubViews.Count;

        shell.NavigateCommand.Execute(StringKeys.NavTest);

        Assert.IsType<TestPageView>(window.CurrentPageView);
        Assert.Contains("Test", window.HeaderPage.Text);

        // Static chrome: same view instances, same content, same number of top level views.
        Assert.Equal(headerBefore, window.HeaderTitle.Text);
        Assert.Equal(footerBefore, window.FooterText.Text);
        Assert.Equal(chromeCountBefore, window.SubViews.Count);
        Assert.Single(window.Content.SubViews);

        provider.Dispose();
    }

    [Fact]
    public void Form_submit_button_mirrors_the_command_and_binding_updates_the_view_model()
    {
        var (provider, shell) = CreateShell();
        using var window = new ShellWindow(shell, new PageViewFactory().Create);

        shell.NavigateCommand.Execute(StringKeys.NavTest);
        var page = Assert.IsType<TestPageView>(window.CurrentPageView);
        var viewModel = page.ViewModel;

        // Nothing is filled in yet, so the shared view model reports an invalid form...
        Assert.False(viewModel.IsFormValid);
        Assert.False(page.SubmitButton.Enabled);
        Assert.Equal(viewModel.FormMessage, page.FormMessage.Text);

        // ...typing through the *view* flows into the view model (two-way binding)...
        page.LoginInput.Value = "user@example.com";
        page.PasswordInput.Value = "Passw0rd!";
        page.ConfirmPasswordInput.Value = "Passw0rd!";

        Assert.Equal("user@example.com", viewModel.Login);
        Assert.True(viewModel.IsFormValid);

        // ...and enabling the shared command enables the TUI button.
        Assert.True(page.SubmitButton.Enabled);
        Assert.Equal(string.Empty, page.LoginError.Text);

        // Wrong data disables it again.
        page.ConfirmPasswordInput.Value = "Passw0rd?";
        Assert.False(page.SubmitButton.Enabled);
        Assert.Equal("The passwords do not match.", page.ConfirmPasswordError.Text);

        provider.Dispose();
    }

    [Fact]
    public void Submit_button_command_goes_through_the_injected_service()
    {
        var (provider, shell) = CreateShell();
        using var window = new ShellWindow(shell, new PageViewFactory().Create);

        shell.NavigateCommand.Execute(StringKeys.NavTest);
        var page = Assert.IsType<TestPageView>(window.CurrentPageView);

        page.LoginInput.Value = "user@example.com";
        page.PasswordInput.Value = "Passw0rd!";
        page.ConfirmPasswordInput.Value = "Passw0rd!";

        page.SubmitButton.InvokeCommand(Command.Accept);

        Assert.Equal("Hello, World! Registration submitted for user@example.com.", page.ViewModel.StatusMessage);
        Assert.Equal(page.ViewModel.StatusMessage, page.StatusMessage.Text);

        provider.Dispose();
    }

    [Fact]
    public void Main_page_uses_the_injected_service_and_reflects_view_model_changes()
    {
        var (provider, shell) = CreateShell();
        using var window = new ShellWindow(shell, new PageViewFactory().Create);

        var page = Assert.IsType<MainPageView>(window.CurrentPageView);

        // IHelloService came from the DI container.
        Assert.Contains(SubViews(page), v => v is Label { Text: "Hello, World!" });

        page.NameInput.Value = "Ada";
        Assert.Equal("Ada", page.ViewModel.Name);
        Assert.Equal("Hello, Ada!", page.PreviewLabel.Text);

        // The counter is driven by a command and its CanExecute gates the reset button.
        Assert.False(page.ResetButton.Enabled);
        page.GreetButton.InvokeCommand(Command.Accept);
        Assert.Equal(1, page.ViewModel.ClickCount);
        Assert.Equal("Button clicked 1 time(s).", page.CounterLabel.Text);
        Assert.True(page.ResetButton.Enabled);

        provider.Dispose();
    }

    [Fact]
    public void Setup_page_switches_the_language_of_the_whole_app()
    {
        var (provider, shell) = CreateShell();
        using var window = new ShellWindow(shell, new PageViewFactory().Create);

        shell.NavigateCommand.Execute(StringKeys.NavSetup);
        var page = Assert.IsType<SetupPageView>(window.CurrentPageView);

        page.NameInput.Value = "Ada";
        Assert.Equal("Hello, Ada!", page.PreviewLabel.Text);

        page.LanguageList.SelectedItem = page.ViewModel.LanguageOptions.ToList().FindIndex(o => o.Culture.Name == "cs");
        page.LanguageList.InvokeCommand(Command.Accept);

        Assert.Equal("cs", provider.GetRequiredService<ILocalizer>().Culture.Name);
        Assert.Equal("Ahoj, Ada!", page.PreviewLabel.Text);
        Assert.Equal("Afutrans Překladač", window.HeaderTitle.Text);

        provider.Dispose();
    }

    private static IEnumerable<View> SubViews(View root)
    {
        foreach(var view in root.SubViews)
        {
            yield return view;

            foreach(var descendant in SubViews(view))
            {
                yield return descendant;
            }
        }
    }
}
