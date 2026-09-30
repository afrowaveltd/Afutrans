using CommunityToolkit.Mvvm.ComponentModel;
using Afutrans.Core.Localization;

namespace Afutrans.Core.ViewModels;

/// <summary>
/// Base class of every shared view model.
/// </summary>
/// <remarks>
/// Derives from <see cref="ObservableValidator"/> so that the MVVM Toolkit source generators
/// provide both change notification (<c>ObservableObject</c>) and validation
/// (<c>INotifyDataErrorInfo</c>) for all front ends. The localizer is injected per instance,
/// so view models never depend on global state.
/// </remarks>
public abstract class ViewModelBase : ObservableValidator
{
    /// <summary>Initializes a new view model.</summary>
    /// <param name="localizer">Localizer used for every user visible string of this view model.</param>
    protected ViewModelBase(ILocalizer localizer) =>
        Strings = localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <summary>Gets the localizer of this view model.</summary>
    protected ILocalizer Strings { get; }

    /// <summary>Shorthand for the localized string with <paramref name="key"/>.</summary>
    protected string L(string key) => Strings[key];

    /// <summary>Shorthand for the formatted localized string with <paramref name="key"/>.</summary>
    protected string L(string key, params object?[] args) => Strings.Format(key, args);
}
