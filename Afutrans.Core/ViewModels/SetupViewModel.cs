using System.Globalization;
using Afutrans.Core.Localization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Afutrans.Core.ViewModels;

/// <summary>
/// The <c>Setup</c> page: a reactive, two-way bound text box plus the language picker that drives
/// the shared <see cref="ILocalizer"/>.
/// </summary>
public sealed partial class SetupViewModel : PageViewModelBase
{
    /// <summary>Initializes a new setup page view model.</summary>
    public SetupViewModel(ILocalizer localizer)
        : base(localizer)
    {
        LanguageOptions = [.. localizer.AvailableCultures.Select(culture => new LanguageOption(culture, culture.NativeName))];
        SelectedLanguage = LanguageOptions.FirstOrDefault(option => option.Culture.Name == localizer.Culture.Name);

        Localizer.CultureChanged += (_, _) =>
        {
            RefreshLocalizedStrings();
            OnPropertyChanged(nameof(CurrentLanguageText));
            OnPropertyChanged(nameof(GreetingPreview));
        };
    }

    /// <inheritdoc />
    public override string Title => Localizer[StringKeys.SetupTitle];

    /// <inheritdoc />
    public override string Description => Localizer[StringKeys.SetupDescription];

    /// <summary>Gets the caption of the name text box.</summary>
    public string NameLabel => Localizer[StringKeys.SetupNameLabel];

    /// <summary>Gets the caption of the greeting preview.</summary>
    public string PreviewLabel => Localizer[StringKeys.SetupPreviewLabel];

    /// <summary>Gets the caption of the language picker.</summary>
    public string LanguageLabel => Localizer[StringKeys.SetupLanguageLabel];

    /// <summary>Gets the hint explaining the reactive binding.</summary>
    public string NameHint => Localizer[StringKeys.SetupLanguageHint];

    /// <summary>Gets or sets the user name; the preview follows every keystroke.</summary>
    [ObservableProperty]
    public partial string UserName { get; set; } = string.Empty;

    /// <summary>Gets the greeting preview built from <see cref="UserName"/>.</summary>
    public string GreetingPreview =>
        string.IsNullOrWhiteSpace(UserName)
            ? Localizer[StringKeys.HelloWorld]
            : Localizer.Format(StringKeys.HelloNamed, UserName.Trim());

    /// <summary>Gets the languages the shared localizer can switch to.</summary>
    public IReadOnlyList<LanguageOption> LanguageOptions { get; }

    /// <summary>Gets or sets the selected language; setting it updates <see cref="ILocalizer"/> for the whole app.</summary>
    [ObservableProperty]
    public partial LanguageOption? SelectedLanguage { get; set; }

    /// <summary>Gets the localized "current language" text shown under the picker.</summary>
    public string CurrentLanguageText =>
        Localizer.Format(StringKeys.SetupCurrentLanguage, SelectedLanguage?.DisplayName ?? Localizer.Culture.NativeName);

    partial void OnUserNameChanged(string value) => OnPropertyChanged(nameof(GreetingPreview));

    partial void OnSelectedLanguageChanged(LanguageOption? value)
    {
        if(value is not null)
        {
            Localizer.SetCulture(value.Culture);
        }

        OnPropertyChanged(nameof(CurrentLanguageText));
    }
}

/// <summary>A selectable UI language.</summary>
/// <param name="Culture">Culture used for lookups.</param>
/// <param name="DisplayName">Human readable name, shown in the picker.</param>
public sealed record LanguageOption(CultureInfo Culture, string DisplayName)
{
    /// <summary>Gets the display text (for example <c>English</c>).</summary>
    public override string ToString() => DisplayName;
}
