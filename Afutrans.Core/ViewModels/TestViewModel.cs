using System.ComponentModel.DataAnnotations;
using Afutrans.Core.Localization;
using Afutrans.Core.Services;
using Afutrans.Core.Validation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Afutrans.Core.ViewModels;

/// <summary>
/// The <c>Test</c> page: a reactive registration form.
/// </summary>
/// <remarks>
/// <para>Every property validates on each change (<c>NotifyDataErrorInfo</c>), the cross-field
/// rule re-runs when either password changes, and <see cref="SubmitCommand"/> is bound to
/// <c>CanExecute</c> — so both front ends automatically disable their submit button until the
/// data is valid.</para>
/// <para>The page also consumes <see cref="IHelloService"/> so the submit handler proves that DI
/// works inside a page that was created by the navigation service.</para>
/// </remarks>
public sealed partial class TestViewModel : PageViewModelBase
{
    private readonly IHelloService _helloService;

    /// <summary>Initializes a new registration form view model.</summary>
    public TestViewModel(ILocalizer localizer, IHelloService helloService)
        : base(localizer)
    {
        _helloService = helloService ?? throw new ArgumentNullException(nameof(helloService));

        SubmitCommand = new RelayCommand(Submit, () => IsFormValid);
        ResetCommand = new RelayCommand(Reset);

        Localizer.CultureChanged += (_, _) =>
        {
            RefreshLocalizedStrings();
            RaiseErrorMessages();
        };
    }

    /// <inheritdoc />
    public override string Title => Localizer[StringKeys.TestTitle];

    /// <inheritdoc />
    public override string Description => Localizer[StringKeys.TestDescription];

    /// <summary>Gets the caption of the login box.</summary>
    public string LoginLabel => Localizer[StringKeys.TestLoginLabel];

    /// <summary>Gets the caption of the password box.</summary>
    public string PasswordLabel => Localizer[StringKeys.TestPasswordLabel];

    /// <summary>Gets the caption of the confirmation box.</summary>
    public string ConfirmPasswordLabel => Localizer[StringKeys.TestConfirmPasswordLabel];

    /// <summary>Gets the caption of the submit button.</summary>
    public string SubmitLabel => Localizer[StringKeys.TestSubmit];

    /// <summary>Gets the caption of the reset button.</summary>
    public string ResetLabel => Localizer[StringKeys.TestReset];

    /// <summary>Gets the localized password policy hint.</summary>
    public string PasswordHint => Localizer.Format(StringKeys.TestPasswordHint, PasswordRules.MinimumLength);

    /// <summary>Gets or sets the login; it has to be a valid e-mail address.</summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredLocalized(StringKeys.ValidationLoginRequired)]
    [EmailLocalized(StringKeys.ValidationLoginInvalid)]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    public partial string Login { get; set; } = string.Empty;

    /// <summary>Gets or sets the password; it has to satisfy <see cref="PasswordRules"/>.</summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredLocalized(StringKeys.ValidationPasswordRequired)]
    [PasswordRulesAttribute(StringKeys.ValidationPasswordLength, StringKeys.ValidationPasswordComplexity)]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    public partial string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets the repeated password; it has to equal <see cref="Password"/>.</summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredLocalized(StringKeys.ValidationPasswordConfirmationRequired)]
    [PasswordConfirmation(nameof(Password), StringKeys.ValidationPasswordConfirmationMismatch)]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    public partial string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>Gets or sets the status text shown after submitting.</summary>
    [ObservableProperty]
    public partial string StatusMessage { get; set; } = string.Empty;

    /// <summary>Gets the first validation message of the login field, or an empty string.</summary>
    public string LoginError => FirstError(nameof(Login));

    /// <summary>Gets the first validation message of the password field, or an empty string.</summary>
    public string PasswordError => FirstError(nameof(Password));

    /// <summary>Gets the first validation message of the confirmation field, or an empty string.</summary>
    public string ConfirmPasswordError => FirstError(nameof(ConfirmPassword));

    /// <summary>Gets a value indicating whether every field is valid — the submit button follows this value.</summary>
    public bool IsFormValid =>
        EmailRules.IsValid(Login) &&
        PasswordRules.IsValid(Password) &&
        !string.IsNullOrEmpty(ConfirmPassword) &&
        string.Equals(Password, ConfirmPassword, StringComparison.Ordinal);

    /// <summary>Gets the localized "valid / invalid" summary of the form.</summary>
    public string FormMessage => IsFormValid ? Localizer[StringKeys.TestFormValid] : Localizer[StringKeys.TestFormInvalid];

    /// <summary>Gets the command that submits the form; it stays disabled while <see cref="IsFormValid"/> is false.</summary>
    public IRelayCommand SubmitCommand { get; }

    /// <summary>Gets the command that clears the form.</summary>
    public IRelayCommand ResetCommand { get; }

    // Cross-field validation: changing the password must re-validate the confirmation too.
    partial void OnPasswordChanged(string value)
    {
        ValidateProperty(ConfirmPassword, nameof(ConfirmPassword));
        OnFormStateChanged();
    }

    partial void OnLoginChanged(string value) => OnFormStateChanged();

    partial void OnConfirmPasswordChanged(string value) => OnFormStateChanged();

    private void OnFormStateChanged()
    {
        OnPropertyChanged(nameof(LoginError));
        OnPropertyChanged(nameof(PasswordError));
        OnPropertyChanged(nameof(ConfirmPasswordError));
        OnPropertyChanged(nameof(IsFormValid));
        OnPropertyChanged(nameof(FormMessage));
    }

    private void Submit() =>
        StatusMessage = $"{_helloService.Greet()} {Localizer.Format(StringKeys.TestSubmitted, Login)}";

    private void Reset()
    {
        Login = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        StatusMessage = string.Empty;
        ClearErrors();
        OnFormStateChanged();
    }

    private string FirstError(string propertyName) =>
        GetErrors(propertyName).OfType<ValidationResult>().FirstOrDefault()?.ErrorMessage ?? string.Empty;

    private void RaiseErrorMessages()
    {
        OnPropertyChanged(nameof(LoginError));
        OnPropertyChanged(nameof(PasswordError));
        OnPropertyChanged(nameof(ConfirmPasswordError));
        OnPropertyChanged(nameof(FormMessage));
        OnPropertyChanged(nameof(PasswordHint));
        OnPropertyChanged(nameof(LoginLabel));
        OnPropertyChanged(nameof(PasswordLabel));
        OnPropertyChanged(nameof(ConfirmPasswordLabel));
        OnPropertyChanged(nameof(SubmitLabel));
        OnPropertyChanged(nameof(ResetLabel));
    }
}
