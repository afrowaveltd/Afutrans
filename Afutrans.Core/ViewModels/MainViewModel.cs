using Afutrans.Core.Localization;
using Afutrans.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Afutrans.Core.ViewModels;

/// <summary>
/// The <c>Main</c> page: shows the <see cref="IHelloService"/> resolved from the DI container,
/// a reactive two-way bound text box and a command with a counter.
/// </summary>
public sealed partial class MainViewModel : PageViewModelBase
{
    private readonly IHelloService _helloService;

    /// <summary>Initializes a new main page view model.</summary>
    public MainViewModel(ILocalizer localizer, IHelloService helloService)
        : base(localizer)
    {
        _helloService = helloService ?? throw new ArgumentNullException(nameof(helloService));

        ClickCommand = new RelayCommand(OnClicked);
        ResetCommand = new RelayCommand(Reset, () => ClickCount > 0);
        Localizer.CultureChanged += (_, _) => RefreshLocalizedStrings(nameof(NameLabel));    }

    /// <inheritdoc />
    public override string Title => Localizer[StringKeys.MainTitle];

    /// <inheritdoc />
    public override string Description => Localizer[StringKeys.MainDescription];

    /// <summary>Gets the caption of the greeting shown on this page.</summary>
    public string GreetingCaption => Localizer[StringKeys.MainGreetingCaption];

    /// <summary>Gets the hint that explains the binding demo.</summary>
    public string ClickHint => Localizer[StringKeys.MainClickHint];

    /// <summary>Gets the caption of the name input.</summary>
    public string NameLabel => Localizer[StringKeys.SetupNameLabel];

    /// <summary>Gets the caption of the demo button.</summary>
    public string ClickButtonLabel => Localizer[StringKeys.MainClickButton];

    /// <summary>Gets the caption of the reset button.</summary>
    public string ResetLabel => Localizer[StringKeys.MainResetButton];

    /// <summary>Gets the text that tells the user the service really came from the DI container.</summary>
    public string ServiceCaption => Localizer[StringKeys.HelloWelcome];

    /// <summary>Gets the greeting produced by <see cref="IHelloService"/> (no name yet).</summary>
    public string GreetingFromService => _helloService.Greet();

    /// <summary>Gets or sets the name typed by the user; the preview updates on every keystroke.</summary>
    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    /// <summary>Gets the greeting preview that follows <see cref="Name"/>.</summary>
    public string GreetingPreview => _helloService.Greet(Name);

    /// <summary>Gets or sets the number of times the demo button was pressed.</summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    public partial int ClickCount { get; set; }

    /// <summary>Gets the localized click counter text.</summary>
    public string ClickCountText => Localizer.Format(StringKeys.MainClickCount, ClickCount);

    /// <summary>Gets the command bound to the demo button.</summary>
    public IRelayCommand ClickCommand { get; }

    /// <summary>Gets the command that resets the counter; it is disabled while the counter is zero.</summary>
    public IRelayCommand ResetCommand { get; }

    partial void OnNameChanged(string value) => OnPropertyChanged(nameof(GreetingPreview));

    partial void OnClickCountChanged(int value) => OnPropertyChanged(nameof(ClickCountText));

    private void OnClicked() => ClickCount++;

    private void Reset() => ClickCount = 0;
}
