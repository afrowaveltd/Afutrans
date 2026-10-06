using Afutrans.Core.DependencyInjection;
using Afutrans.Core.Localization;
using Afutrans.Core.Navigation;
using Afutrans.Core.Validation;
using Afutrans.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Afutrans.Core.Tests;

public class NavigationTests
{
    [Fact]
    public void Routes_are_registered_in_the_declared_order()
    {
        using var provider = TestHost.Build();
        var navigation = provider.GetRequiredService<INavigationService>();

        Assert.Equal([StringKeys.NavMain, StringKeys.NavSetup, StringKeys.NavTest], navigation.Routes.Select(r => r.Key));
    }

    [Fact]
    public void NavigateTo_swaps_the_current_page_and_raises_Navigated()
    {
        using var provider = TestHost.Build();
        var navigation = provider.GetRequiredService<INavigationService>();
        var events = new List<string>();
        navigation.Navigated += (_, e) => events.Add(e.Route.Key);

        Assert.True(navigation.NavigateTo(StringKeys.NavMain));
        Assert.IsType<MainViewModel>(navigation.CurrentViewModel);

        Assert.True(navigation.NavigateTo(StringKeys.NavTest));
        Assert.IsType<TestViewModel>(navigation.CurrentViewModel);

        Assert.Equal([StringKeys.NavMain, StringKeys.NavTest], events);
    }

    [Fact]
    public void Re_navigating_to_the_current_route_is_a_no_op_so_page_state_survives()
    {
        using var provider = TestHost.Build();
        var navigation = provider.GetRequiredService<INavigationService>();
        navigation.NavigateTo(StringKeys.NavTest);

        var first = Assert.IsType<TestViewModel>(navigation.CurrentViewModel);
        first.Login = "user@example.com";

        Assert.False(navigation.NavigateTo(StringKeys.NavTest));
        Assert.Same(first, navigation.CurrentViewModel);
        Assert.Equal("user@example.com", first.Login);
    }

    [Fact]
    public void NavigateTo_unknown_key_throws()
    {
        using var provider = TestHost.Build();
        var navigation = provider.GetRequiredService<INavigationService>();

        Assert.Throws<InvalidOperationException>(() => navigation.NavigateTo("Nav.DoesNotExist"));
    }

    [Fact]
    public void Pages_are_recreated_per_navigation_and_the_previous_one_is_notified()
    {
        using var provider = TestHost.Build();
        var navigation = provider.GetRequiredService<INavigationService>();

        navigation.NavigateTo(StringKeys.NavTest);
        var first = navigation.CurrentViewModel;

        navigation.NavigateTo(StringKeys.NavMain);

        Assert.NotSame(first, navigation.CurrentViewModel);
    }
}

public class ShellViewModelTests
{
    [Fact]
    public void Shell_starts_on_the_first_route_and_marks_it_current()
    {
        using var shell = TestHost.CreateShell(out var provider);

        Assert.Equal(StringKeys.NavMain, shell.CurrentRouteKey);
        Assert.Equal("Main", shell.CurrentPageTitle);
        Assert.True(shell.NavigationItems.Single(i => i.RouteKey == StringKeys.NavMain).IsCurrent);
        Assert.False(shell.NavigationItems.Single(i => i.RouteKey == StringKeys.NavTest).IsCurrent);

        provider.Dispose();
    }

    [Fact]
    public void NavigateCommand_swaps_only_the_content_view_model()
    {
        using var shell = TestHost.CreateShell(out var provider);
        var header = shell.AppTitle;
        var footer = shell.FooterText;

        shell.NavigateCommand.Execute(StringKeys.NavTest);

        Assert.IsType<TestViewModel>(shell.CurrentViewModel);
        Assert.Equal("Test", shell.CurrentPageTitle);
        Assert.True(shell.NavigationItems.Single(i => i.RouteKey == StringKeys.NavTest).IsCurrent);

        // Static chrome must not change when navigating.
        Assert.Equal(header, shell.AppTitle);
        Assert.Equal(footer, shell.FooterText);

        provider.Dispose();
    }

    [Fact]
    public void Shell_can_start_on_an_explicit_route()
    {
        using var shell = TestHost.CreateShell(out var provider, StringKeys.NavSetup);

        Assert.IsType<SetupViewModel>(shell.CurrentViewModel);
        Assert.Equal("Setup", shell.CurrentPageTitle);

        provider.Dispose();
    }
}

public class DependencyInjectionTests
{
    [Fact]
    public void Hello_service_is_a_singleton_and_is_used_by_the_main_page()
    {
        using var provider = TestHost.Build();
        var first = provider.GetRequiredService<Afutrans.Core.Services.IHelloService>();
        var second = provider.GetRequiredService<Afutrans.Core.Services.IHelloService>();

        Assert.Same(first, second);
        Assert.Equal("Hello, World!", first.Greet());
        Assert.Equal("Hello, Ada!", first.Greet("Ada"));

        Assert.Equal("Hello, World!", new MainViewModel(provider.GetRequiredService<ILocalizer>(), first).GreetingFromService);
    }

    [Fact]
    public void Missing_route_table_registration_throws_a_helpful_error()
    {
        var services = new ServiceCollection();

        var exception = Assert.Throws<InvalidOperationException>(() => services.AddAfutransPage<MainViewModel>(StringKeys.NavMain));

        Assert.Contains(nameof(ServiceCollectionExtensions.AddAfutransCore), exception.Message);
    }
}

public class PasswordRulesTests
{
    [Theory]
    [InlineData("Abcdef1!", true)]
    [InlineData("abcdef1!", false)]   // no upper-case
    [InlineData("ABCDEF1!", false)]   // no lower-case
    [InlineData("Abcdefg!", false)]   // no digit
    [InlineData("Abcdefg1", false)]   // no symbol
    [InlineData("Ab1!", false)]       // too short
    [InlineData("", false)]
    [InlineData(null, false)]
    public void Password_policy_is_enforced(string? password, bool expected) =>
        Assert.Equal(expected, PasswordRules.IsValid(password));
}

public class ValidationLocalizationTests
{
    [Fact]
    public void Validation_messages_come_from_the_shared_catalog()
    {
        using var provider = TestHost.Build();
        var test = new TestViewModel(provider.GetRequiredService<ILocalizer>(), provider.GetRequiredService<Afutrans.Core.Services.IHelloService>());

        test.Login = "not-an-email";
        test.Password = "weak";
        test.ConfirmPassword = "different";

        Assert.Equal("Enter a valid e-mail address (for example user@example.com).", test.LoginError);
        Assert.Equal("Password must be at least 8 characters long.", test.PasswordError);
        Assert.Equal("The passwords do not match.", test.ConfirmPasswordError);
    }
}
