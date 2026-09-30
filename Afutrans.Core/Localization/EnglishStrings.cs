namespace Afutrans.Core.Localization;

/// <summary>
/// Default (English) translation catalog. English is the fallback for every other culture,
/// so a missing key must never crash a front end — it falls back to this catalog and finally
/// to the key itself.
/// </summary>
public static class EnglishStrings
{
    /// <summary>Gets the default English strings.</summary>
    public static IReadOnlyDictionary<string, string> Values { get; } = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        // Application shell
        [StringKeys.AppTitle] = "Afutrans Translator",
        [StringKeys.AppFooter] = "Shared core demo: MVVM Toolkit · Dependency Injection · Navigation · Localization",

        // Navigation
        [StringKeys.NavMain] = "Main",
        [StringKeys.NavSetup] = "Setup",
        [StringKeys.NavTest] = "Test",

        // Main page
        [StringKeys.MainTitle] = "Main",
        [StringKeys.MainDescription] = "Welcome! This page, its view model and its services come from the shared Afutrans.Core library.",
        [StringKeys.MainGreetingCaption] = "IHelloService says",
        [StringKeys.MainClickHint] = "Press the button: the label is bound to the view model and the count is a RelayCommand.",
        [StringKeys.MainClickCount] = "Button clicked {0} time(s).",
        [StringKeys.MainClickButton] = "_Greet me",
        [StringKeys.MainResetButton] = "_Reset",

        // Setup page
        [StringKeys.SetupTitle] = "Setup",
        [StringKeys.SetupDescription] = "Pick the application language. The shared localizer tells every front end to re-read its strings.",
        [StringKeys.SetupNameLabel] = "Your name",
        [StringKeys.SetupPreviewLabel] = "Greeting preview",
        [StringKeys.SetupLanguageLabel] = "Language",
        [StringKeys.SetupLanguageHint] = "Type a name — the greeting preview updates on every keystroke (reactive binding).",
        [StringKeys.SetupCurrentLanguage] = "Current language: {0}",

        // Test page (registration form)
        [StringKeys.TestTitle] = "Test",
        [StringKeys.TestDescription] = "Registration form: every keystroke is validated and the Register button is bound to CanExecute.",
        [StringKeys.TestLoginLabel] = "Login (e-mail)",
        [StringKeys.TestPasswordLabel] = "Password",
        [StringKeys.TestConfirmPasswordLabel] = "Confirm password",
        [StringKeys.TestSubmit] = "Register",
        [StringKeys.TestReset] = "Reset",
        [StringKeys.TestSubmitted] = "Registration submitted for {0}.",
        [StringKeys.TestPasswordHint] = "Rules: at least {0} characters with an upper-case letter, a lower-case letter, a digit and a symbol.",
        [StringKeys.TestFormValid] = "All fields are valid — the form can be submitted.",
        [StringKeys.TestFormInvalid] = "The form is invalid — the Register button stays disabled.",

        // Practice service
        [StringKeys.HelloWorld] = "Hello, World!",
        [StringKeys.HelloNamed] = "Hello, {0}!",
        [StringKeys.HelloWelcome] = "Injected from the DI container as IHelloService.",

        // Validation messages
        [StringKeys.ValidationLoginRequired] = "E-mail is required.",
        [StringKeys.ValidationLoginInvalid] = "Enter a valid e-mail address (for example user@example.com).",
        [StringKeys.ValidationPasswordRequired] = "Password is required.",
        [StringKeys.ValidationPasswordLength] = "Password must be at least {0} characters long.",
        [StringKeys.ValidationPasswordComplexity] = "Password needs an upper-case letter, a lower-case letter, a digit and a symbol.",
        [StringKeys.ValidationPasswordConfirmationRequired] = "Please repeat the password.",
        [StringKeys.ValidationPasswordConfirmationMismatch] = "The passwords do not match.",
    };
}
