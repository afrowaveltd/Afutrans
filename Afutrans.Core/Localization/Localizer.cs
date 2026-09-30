using System.Globalization;

namespace Afutrans.Core.Localization;

/// <summary>
/// Dictionary based <see cref="ILocalizer"/>: one catalog per culture with English as fallback.
/// </summary>
public sealed class Localizer : ILocalizer
{
    private readonly Dictionary<string, IReadOnlyDictionary<string, string>> _catalogs;
    private readonly List<CultureInfo> _availableCultures;
    private CultureInfo _culture;

    /// <summary>Initializes a new localizer.</summary>
    /// <param name="catalogs">Catalog per culture name (for example <c>en</c>); English is required.</param>
    /// <param name="culture">Culture to start with; defaults to English.</param>
    public Localizer(IDictionary<string, IReadOnlyDictionary<string, string>> catalogs, CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(catalogs);

        _catalogs = new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        foreach(var (name, catalog) in catalogs)
        {
            _catalogs[name] = catalog;
        }

        if(!_catalogs.ContainsKey(FallbackCultureName))
        {
            throw new ArgumentException($"A catalog for the fallback culture '{FallbackCultureName}' is required.", nameof(catalogs));
        }

        _availableCultures = [.. _catalogs.Keys.Select(name => CultureInfo.GetCultureInfo(name)).OrderBy(c => c.Name, StringComparer.Ordinal)];
        _culture = culture ?? CultureInfo.GetCultureInfo(FallbackCultureName);
    }

    /// <summary>Gets the culture name that is used when a translation is missing.</summary>
    public static string FallbackCultureName => "en";

    /// <inheritdoc />
    public CultureInfo Culture => _culture;

    /// <inheritdoc />
    public IReadOnlyList<CultureInfo> AvailableCultures => _availableCultures;

    /// <inheritdoc />
    public event EventHandler<CultureChangedEventArgs>? CultureChanged;

    /// <inheritdoc />
    public string this[string key]
    {
        get
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            return Lookup(_culture, key)
                   ?? Lookup(CultureInfo.GetCultureInfo(FallbackCultureName), key)
                   ?? (EnglishStrings.Values.TryGetValue(key, out var builtIn) ? builtIn : key);
        }
    }

    /// <inheritdoc />
    public string Format(string key, params object?[] args)
    {
        var template = this[key];

        return args is null || args.Length == 0 ? template : string.Format(_culture, template, args);
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        if(string.Equals(_culture.Name, culture.Name, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _culture = culture;
        CultureChanged?.Invoke(this, new CultureChangedEventArgs(culture));
    }

    /// <summary>Creates the default localizer with the built-in English (fallback) and Czech catalogs.</summary>
    /// <param name="culture">Culture to start with; defaults to English.</param>
    public static Localizer CreateDefault(System.Globalization.CultureInfo? culture = null) =>
        new(new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            [FallbackCultureName] = EnglishStrings.Values,
            ["cs"] = CzechStrings.Values,
        }, culture);

    private string? Lookup(CultureInfo culture, string key)
    {
        if(_catalogs.TryGetValue(culture.Name, out var exact) && exact.TryGetValue(key, out var value))
        {
            return value;
        }

        if(!string.IsNullOrEmpty(culture.Parent.Name)
           && _catalogs.TryGetValue(culture.Parent.Name, out var parent)
           && parent.TryGetValue(key, out var parentValue))
        {
            return parentValue;
        }

        return null;
    }
}
