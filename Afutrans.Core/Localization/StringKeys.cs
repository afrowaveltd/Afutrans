namespace Afutrans.Core.Localization;

/// <summary>
/// Keys used to look up localized strings.
/// </summary>
/// <remarks>
/// These keys are the contract between the shared view models and the translation catalogs.
/// Keep the value of a key stable once it has been translated; only the catalog changes.
/// </remarks>
public static class StringKeys
{
    // --- Application shell -------------------------------------------------
    public const string AppTitle = "App.Title";
    public const string AppFooter = "App.Footer";

    // --- Navigation --------------------------------------------------------
    public const string NavMain = "Nav.Main";
    public const string NavSetup = "Nav.Setup";
    public const string NavTest = "Nav.Test";

    // --- Main page ---------------------------------------------------------
    public const string MainTitle = "Main.Title";
    public const string MainDescription = "Main.Description";
    public const string MainGreetingCaption = "Main.GreetingCaption";
    public const string MainClickHint = "Main.ClickHint";
    public const string MainClickCount = "Main.ClickCount";
    public const string MainClickButton = "Main.ClickButton";
    public const string MainResetButton = "Main.ResetButton";

    // --- Setup page --------------------------------------------------------
    public const string SetupTitle = "Setup.Title";
    public const string SetupDescription = "Setup.Description";
    public const string SetupNameLabel = "Setup.NameLabel";
    public const string SetupPreviewLabel = "Setup.PreviewLabel";
    public const string SetupLanguageLabel = "Setup.LanguageLabel";
    public const string SetupLanguageHint = "Setup.LanguageHint";
    public const string SetupCurrentLanguage = "Setup.CurrentLanguage";

    // --- Test page (registration form) ------------------------------------
    public const string TestTitle = "Test.Title";
    public const string TestDescription = "Test.Description";
    public const string TestLoginLabel = "Test.LoginLabel";
    public const string TestPasswordLabel = "Test.PasswordLabel";
    public const string TestConfirmPasswordLabel = "Test.ConfirmPasswordLabel";
    public const string TestSubmit = "Test.Submit";
    public const string TestReset = "Test.Reset";
    public const string TestSubmitted = "Test.Submitted";
    public const string TestPasswordHint = "Test.PasswordHint";
    public const string TestFormValid = "Test.FormValid";
    public const string TestFormInvalid = "Test.FormInvalid";

    // --- Practice service --------------------------------------------------
    public const string HelloWorld = "Hello.World";
    public const string HelloNamed = "Hello.Named";
    public const string HelloWelcome = "Hello.Welcome";

    // --- Validation messages ----------------------------------------------
    public const string ValidationLoginRequired = "Validation.Login.Required";
    public const string ValidationLoginInvalid = "Validation.Login.Invalid";
    public const string ValidationPasswordRequired = "Validation.Password.Required";
    public const string ValidationPasswordLength = "Validation.Password.Length";
    public const string ValidationPasswordComplexity = "Validation.Password.Complexity";
    public const string ValidationPasswordConfirmationRequired = "Validation.PasswordConfirmation.Required";
    public const string ValidationPasswordConfirmationMismatch = "Validation.PasswordConfirmation.Mismatch";
}
