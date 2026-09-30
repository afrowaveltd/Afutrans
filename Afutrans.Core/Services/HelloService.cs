using Afutrans.Core.Localization;

namespace Afutrans.Core.Services;

/// <summary>Default <see cref="IHelloService"/> implementation.</summary>
public sealed class HelloService(ILocalizer localizer) : IHelloService
{
    private readonly ILocalizer _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <inheritdoc />
    public string Greet() => _localizer[StringKeys.HelloWorld];

    /// <inheritdoc />
    public string Greet(string? name) =>
        string.IsNullOrWhiteSpace(name)
            ? Greet()
            : _localizer.Format(StringKeys.HelloNamed, name.Trim());
}
