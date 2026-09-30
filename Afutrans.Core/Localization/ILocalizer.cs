using System.Globalization;

namespace Afutrans.Core.Localization;

/// <summary>
/// Provides localized strings to the shared view models.
/// </summary>
/// <remarks>
/// The implementation is shared by every front end (desktop GUI, TUI, tests) so all of them
/// show exactly the same texts. Views must never hard-code UI text — they bind to view model
/// properties that are refreshed when <see cref="CultureChanged"/> is raised.
/// </remarks>
public interface ILocalizer
{
    /// <summary>Gets the culture currently used for lookups.</summary>
    CultureInfo Culture { get; }

    /// <summary>Gets the cultures that have a translation catalog loaded.</summary>
    IReadOnlyList<CultureInfo> AvailableCultures { get; }

    /// <summary>Gets the translated string for <paramref name="key"/>; falls back to English and finally to the key.</summary>
    string this[string key] { get; }

    /// <summary>Gets the translated string for <paramref name="key"/> with <paramref name="args"/> applied.</summary>
    string Format(string key, params object?[] args);

    /// <summary>Raised after <see cref="SetCulture"/> selected another culture.</summary>
    event EventHandler<CultureChangedEventArgs>? CultureChanged;

    /// <summary>Selects the culture used for subsequent lookups; does nothing when it is already active.</summary>
    void SetCulture(CultureInfo culture);
}

/// <summary>Event data for <see cref="ILocalizer.CultureChanged"/>.</summary>
public sealed class CultureChangedEventArgs(CultureInfo culture) : EventArgs
{
    /// <summary>Gets the culture that is now active.</summary>
    public CultureInfo Culture { get; } = culture;
}
