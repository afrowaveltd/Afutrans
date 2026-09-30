using Afutrans.Core.Localization;
using Afutrans.Core.Navigation;

namespace Afutrans.Core.ViewModels;

/// <summary>
/// Base class for the pages that are hosted by the application shell.
/// </summary>
public abstract class PageViewModelBase : ViewModelBase, INavigableViewModel
{
    /// <summary>Initializes a new page view model.</summary>
    protected PageViewModelBase(ILocalizer localizer)
        : base(localizer)
    {
        Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    /// <summary>Gets the localizer injected into this page.</summary>
    protected ILocalizer Localizer { get; }

    /// <summary>Gets the localized page title (shown in the header).</summary>
    public abstract string Title { get; }

    /// <summary>Gets the localized page description.</summary>
    public abstract string Description { get; }

    /// <inheritdoc />
    public virtual void OnNavigatedTo() => OnPropertyChanged(nameof(Title));

    /// <inheritdoc />
    public virtual void OnNavigatedFrom()
    {
    }

    /// <summary>Raises <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> for all localized properties.</summary>
    protected void RefreshLocalizedStrings(string? titleProperty = null)
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Description));

        if(titleProperty is not null)
        {
            OnPropertyChanged(titleProperty);
        }
    }
}
