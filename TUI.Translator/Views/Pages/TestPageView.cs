using Afutrans.Core.ViewModels;
using TUI.Translator.Binding;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TUI.Translator.Views.Pages;

/// <summary>
/// The <c>Test</c> page of the TUI: the reactive registration form.
/// </summary>
/// <remarks>
/// Every text box is two-way bound to the shared <see cref="TestViewModel"/>, so the validation
/// that runs on each change is visible immediately: the error labels refresh, the summary tells
/// whether the form is valid, and the Register button is enabled or disabled by
/// <c>SubmitCommand.CanExecute</c> — exactly like in the Avalonia front end.
/// </remarks>
public sealed class TestPageView : View
{
    private readonly List<IDisposable> _bindings = [];

    /// <summary>Initializes the registration form view.</summary>
    public TestPageView(TestViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        Width = Dim.Fill();
        Height = Dim.Fill();

        Label title = new() { X = 1, Y = 0, Text = viewModel.Title };
        Label description = new() { X = 1, Y = 1, Width = Dim.Fill(), Text = viewModel.Description };

        Label loginLabel = new() { X = 1, Y = 3, Text = viewModel.LoginLabel };
        LoginInput = new TextField { X = 1, Y = 4, Width = 40, Text = viewModel.Login };
        LoginError = new Label { X = 1, Y = 5, Width = Dim.Fill() };

        Label passwordLabel = new() { X = 1, Y = 7, Text = viewModel.PasswordLabel };
        PasswordInput = new TextField { X = 1, Y = 8, Width = 40, Secret = true, Text = viewModel.Password };
        PasswordHint = new Label { X = 1, Y = 9, Width = Dim.Fill(), Text = viewModel.PasswordHint };
        PasswordError = new Label { X = 1, Y = 10, Width = Dim.Fill() };

        Label confirmLabel = new() { X = 1, Y = 12, Text = viewModel.ConfirmPasswordLabel };
        ConfirmPasswordInput = new TextField { X = 1, Y = 13, Width = 40, Secret = true, Text = viewModel.ConfirmPassword };
        ConfirmPasswordError = new Label { X = 1, Y = 14, Width = Dim.Fill() };

        FormMessage = new Label { X = 1, Y = 16, Width = Dim.Fill(), Text = viewModel.FormMessage };

        SubmitButton = new Button { X = 1, Y = 18, Text = viewModel.SubmitLabel, IsDefault = true };
        ResetButton = new Button { X = Pos.Right(SubmitButton) + 2, Y = 18, Text = viewModel.ResetLabel };

        StatusMessage = new Label { X = 1, Y = 20, Width = Dim.Fill() };

        SubmitButton.Accepted += (_, _) => viewModel.SubmitCommand.Execute(null);
        ResetButton.Accepted += (_, _) => viewModel.ResetCommand.Execute(null);

        _bindings.Add(ViewBinder.TwoWay<string>(LoginInput, viewModel, nameof(TestViewModel.Login)));
        _bindings.Add(ViewBinder.TwoWay<string>(PasswordInput, viewModel, nameof(TestViewModel.Password)));
        _bindings.Add(ViewBinder.TwoWay<string>(ConfirmPasswordInput, viewModel, nameof(TestViewModel.ConfirmPassword)));

        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(TestViewModel.LoginError), value => LoginError.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(TestViewModel.PasswordError), value => PasswordError.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(TestViewModel.ConfirmPasswordError), value => ConfirmPasswordError.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(TestViewModel.FormMessage), value => FormMessage.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(TestViewModel.StatusMessage), value => StatusMessage.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(TestViewModel.Title), value => title.Text = value));

        // The heart of the demo: the button mirrors the command's CanExecute.
        _bindings.Add(ViewBinder.Command(viewModel.SubmitCommand, SubmitButton));

        Add(
            title,
            description,
            loginLabel,
            LoginInput,
            LoginError,
            passwordLabel,
            PasswordInput,
            PasswordHint,
            PasswordError,
            confirmLabel,
            ConfirmPasswordInput,
            ConfirmPasswordError,
            FormMessage,
            SubmitButton,
            ResetButton,
            StatusMessage);
    }

    /// <summary>Gets the view model this view is bound to.</summary>
    public TestViewModel ViewModel { get; }

    /// <summary>Gets the login input.</summary>
    public TextField LoginInput { get; }

    /// <summary>Gets the validation message of the login input.</summary>
    public Label LoginError { get; }

    /// <summary>Gets the password input.</summary>
    public TextField PasswordInput { get; }

    /// <summary>Gets the password policy hint.</summary>
    public Label PasswordHint { get; }

    /// <summary>Gets the validation message of the password input.</summary>
    public Label PasswordError { get; }

    /// <summary>Gets the password confirmation input.</summary>
    public TextField ConfirmPasswordInput { get; }

    /// <summary>Gets the validation message of the confirmation input.</summary>
    public Label ConfirmPasswordError { get; }

    /// <summary>Gets the label that summarizes whether the form is valid.</summary>
    public Label FormMessage { get; }

    /// <summary>Gets the submit button; it is disabled until the form is valid.</summary>
    public Button SubmitButton { get; }

    /// <summary>Gets the reset button.</summary>
    public Button ResetButton { get; }

    /// <summary>Gets the label that shows the result of a submission.</summary>
    public Label StatusMessage { get; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            foreach(var binding in _bindings)
            {
                binding.Dispose();
            }

            _bindings.Clear();
        }

        base.Dispose(disposing);
    }
}
