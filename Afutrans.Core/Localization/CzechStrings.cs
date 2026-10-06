namespace Afutrans.Core.Localization;

/// <summary>
/// Czech translation catalog — the proof that localization works end to end.
/// </summary>
/// <remarks>
/// Adding another language means adding one catalog like this one and listing it in
/// <see cref="Localizer.CreateDefault"/>. English stays the fallback, so a partially
/// translated catalog is always safe.
/// </remarks>
public static class CzechStrings
{
    /// <summary>Gets the Czech strings.</summary>
    public static IReadOnlyDictionary<string, string> Values { get; } = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        // Application shell
        [StringKeys.AppTitle] = "Afutrans Překladač",
        [StringKeys.AppFooter] = "Ukázka sdíleného jádra: MVVM Toolkit · Dependency Injection · Navigace · Lokalizace",

        // Navigation
        [StringKeys.NavMain] = "Hlavní",
        [StringKeys.NavSetup] = "Nastavení",
        [StringKeys.NavTest] = "Test",

        // Main page
        [StringKeys.MainTitle] = "Hlavní",
        [StringKeys.MainDescription] = "Vítejte! Tato stránka, její view model i služby pocházejí ze sdílené knihovny Afutrans.Core.",
        [StringKeys.MainGreetingCaption] = "IHelloService říká",
        [StringKeys.MainClickHint] = "Stiskněte tlačítko: popisek je svázán s view modelem a počítadlo je RelayCommand.",
        [StringKeys.MainClickCount] = "Tlačítko stisknuto {0}×.",
        [StringKeys.MainClickButton] = "_Pozdrav mě",
        [StringKeys.MainResetButton] = "_Vymazat",

        // Setup page
        [StringKeys.SetupTitle] = "Nastavení",
        [StringKeys.SetupDescription] = "Zvolte jazyk aplikace. Sdílený lokalizátor řekne oběma front endům, aby si přečetly své texty znovu.",
        [StringKeys.SetupNameLabel] = "Vaše jméno",
        [StringKeys.SetupPreviewLabel] = "Náhled pozdravu",
        [StringKeys.SetupLanguageLabel] = "Jazyk",
        [StringKeys.SetupLanguageHint] = "Napište jméno — náhled pozdravu se mění při každém stisku klávesy (reaktivní binding).",
        [StringKeys.SetupCurrentLanguage] = "Aktuální jazyk: {0}",

        // Test page (registration form)
        [StringKeys.TestTitle] = "Test",
        [StringKeys.TestDescription] = "Registrační formulář: každá změna se validuje a tlačítko Registrovat je svázáno s CanExecute.",
        [StringKeys.TestLoginLabel] = "Login (e-mail)",
        [StringKeys.TestPasswordLabel] = "Heslo",
        [StringKeys.TestConfirmPasswordLabel] = "Heslo znovu",
        [StringKeys.TestSubmit] = "Registrovat",
        [StringKeys.TestReset] = "Vymazat",
        [StringKeys.TestSubmitted] = "Registrace odeslána pro {0}.",
        [StringKeys.TestPasswordHint] = "Pravidla: alespoň {0} znaků, velké písmeno, malé písmeno, číslice a znak.",
        [StringKeys.TestFormValid] = "Všechna pole jsou v pořádku — formulář lze odeslat.",
        [StringKeys.TestFormInvalid] = "Formulář není platný — tlačítko Registrovat zůstává zakázané.",

        // Practice service
        [StringKeys.HelloWorld] = "Ahoj, světe!",
        [StringKeys.HelloNamed] = "Ahoj, {0}!",
        [StringKeys.HelloWelcome] = "Vloženo z DI kontejneru jako IHelloService.",

        // Validation messages
        [StringKeys.ValidationLoginRequired] = "E-mail je povinný.",
        [StringKeys.ValidationLoginInvalid] = "Zadejte platný e-mail (například user@example.com).",
        [StringKeys.ValidationPasswordRequired] = "Heslo je povinné.",
        [StringKeys.ValidationPasswordLength] = "Heslo musí mít alespoň {0} znaků.",
        [StringKeys.ValidationPasswordComplexity] = "Heslo musí obsahovat velké písmeno, malé písmeno, číslici a znak.",
        [StringKeys.ValidationPasswordConfirmationRequired] = "Zopakujte prosím heslo.",
        [StringKeys.ValidationPasswordConfirmationMismatch] = "Hesla se neshodují.",
    };
}
