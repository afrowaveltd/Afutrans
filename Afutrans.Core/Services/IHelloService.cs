namespace Afutrans.Core.Services;

/// <summary>
/// Practice service used to demonstrate dependency injection.
/// </summary>
/// <remarks>
/// Registered by <c>AddAfutransCore()</c> and consumed by the shared view models, so both the
/// desktop and the console front end use the very same instance contract.
/// </remarks>
public interface IHelloService
{
    /// <summary>Gets a localized "hello" greeting without a name.</summary>
    string Greet();

    /// <summary>Gets a localized "hello" greeting for <paramref name="name"/>.</summary>
    string Greet(string? name);
}
