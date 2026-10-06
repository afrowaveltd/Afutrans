using System.ComponentModel;
using Afutrans.Core.DependencyInjection;
using Afutrans.Core.Localization;
using Afutrans.Core.Services;
using Afutrans.Core.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Afutrans.Core.Tests;

public class TestViewModelTests
{
    private static TestViewModel Create(ServiceProvider provider) =>
        new(provider.GetRequiredService<ILocalizer>(), provider.GetRequiredService<IHelloService>());

    [Fact]
    public void Form_starts_invalid_and_the_submit_command_is_disabled()
    {
        using var provider = TestHost.Build();
        var vm = Create(provider);

        Assert.False(vm.IsFormValid);
        Assert.False(vm.SubmitCommand.CanExecute(null));
        Assert.Equal("The form is invalid — the Register button stays disabled.", vm.FormMessage);
        Assert.Empty(vm.StatusMessage);
    }

    [Theory]
    [InlineData("user@example.com", "Passw0rd!", "Passw0rd!", true)]
    [InlineData("user@example.com", "Passw0rd!", "Passw0rd?", false)]  // mismatch
    [InlineData("user@example.com", "weak", "weak", false)]            // weak password
    [InlineData("nope", "Passw0rd!", "Passw0rd!", false)]              // bad e-mail
    [InlineData("", "", "", false)]                                    // empty
    public void Submit_is_enabled_exactly_when_every_rule_passes(string login, string password, string confirm, bool expected)
    {
        using var provider = TestHost.Build();
        var vm = Create(provider);

        vm.Login = login;
        vm.Password = password;
        vm.ConfirmPassword = confirm;

        Assert.Equal(expected, vm.IsFormValid);
        Assert.Equal(expected, vm.SubmitCommand.CanExecute(null));
    }

    [Fact]
    public void Validation_runs_on_every_change_and_clears_again()
    {
        using var provider = TestHost.Build();
        var vm = Create(provider);

        vm.Login = "bad";
        Assert.NotEqual(string.Empty, vm.LoginError);

        vm.Login = "user@example.com";
        Assert.Equal(string.Empty, vm.LoginError);

        vm.Password = "short";
        Assert.NotEqual(string.Empty, vm.PasswordError);

        vm.Password = "Passw0rd!";
        Assert.Equal(string.Empty, vm.PasswordError);
    }

    [Fact]
    public void Submit_reports_that_it_worked_and_reset_clears_the_form()
    {
        using var provider = TestHost.Build();
        var vm = Create(provider);

        vm.Login = "user@example.com";
        vm.Password = "Passw0rd!";
        vm.ConfirmPassword = "Passw0rd!";
        Assert.True(vm.SubmitCommand.CanExecute(null));

        vm.SubmitCommand.Execute(null);

        Assert.Equal("Hello, World! Registration submitted for user@example.com.", vm.StatusMessage);

        vm.ResetCommand.Execute(null);

        Assert.Empty(vm.Login);
        Assert.Empty(vm.Password);
        Assert.Empty(vm.StatusMessage);
        Assert.False(vm.IsFormValid);
    }

    [Fact]
    public void Changing_the_password_revalidates_the_confirmation()
    {
        using var provider = TestHost.Build();
        var vm = Create(provider);

        vm.Password = "Passw0rd!";
        vm.ConfirmPassword = "Passw0rd!";
        Assert.Equal(string.Empty, vm.ConfirmPasswordError);

        vm.Password = "Passw0rd?";

        Assert.Equal("The passwords do not match.", vm.ConfirmPasswordError);
        Assert.False(vm.SubmitCommand.CanExecute(null));
    }

    [Fact]
    public void Form_raises_property_changed_so_any_front_end_can_react()
    {
        using var provider = TestHost.Build();
        var vm = Create(provider);
        var changed = new List<string?>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.Login = "user@example.com";

        Assert.Contains(nameof(TestViewModel.Login), changed);
        Assert.Contains(nameof(TestViewModel.IsFormValid), changed);
        Assert.Contains(nameof(TestViewModel.FormMessage), changed);
    }

    [Fact]
    public void Submit_command_notifies_about_can_execute_changes()
    {
        using var provider = TestHost.Build();
        var vm = Create(provider);
        var notified = 0;
        vm.SubmitCommand.CanExecuteChanged += (_, _) => notified++;

        vm.Login = "user@example.com";
        vm.Password = "Passw0rd!";
        vm.ConfirmPassword = "Passw0rd!";

        Assert.True(notified > 0);
    }
}

public class MainAndSetupViewModelTests
{
    [Fact]
    public void Main_greeting_preview_follows_the_name_reactively()
    {
        using var provider = TestHost.Build();
        var vm = new MainViewModel(provider.GetRequiredService<ILocalizer>(), provider.GetRequiredService<IHelloService>());

        Assert.Equal("Hello, World!", vm.GreetingPreview);

        var changed = new List<string?>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.Name = "Ada";

        Assert.Equal("Hello, Ada!", vm.GreetingPreview);
        Assert.Contains(nameof(MainViewModel.GreetingPreview), changed);
    }

    [Fact]
    public void Main_click_counter_uses_commands_and_gates_reset()
    {
        using var provider = TestHost.Build();
        var vm = new MainViewModel(provider.GetRequiredService<ILocalizer>(), provider.GetRequiredService<IHelloService>());

        Assert.False(vm.ResetCommand.CanExecute(null));

        vm.ClickCommand.Execute(null);
        vm.ClickCommand.Execute(null);

        Assert.Equal(2, vm.ClickCount);
        Assert.Equal("Button clicked 2 time(s).", vm.ClickCountText);
        Assert.True(vm.ResetCommand.CanExecute(null));

        vm.ResetCommand.Execute(null);

        Assert.Equal(0, vm.ClickCount);
    }

    [Fact]
    public void Setup_language_picker_drives_the_shared_localizer()
    {
        using var provider = TestHost.Build();
        var vm = new SetupViewModel(provider.GetRequiredService<ILocalizer>());
        var localizer = provider.GetRequiredService<ILocalizer>();

        vm.SelectedLanguage = vm.LanguageOptions.Single(o => o.Culture.Name == "cs");

        Assert.Equal("cs", localizer.Culture.Name);
        Assert.Equal("Nastavení", vm.Title);
    }
}
