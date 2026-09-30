namespace Afutrans.Core.Validation;

/// <summary>
/// Single source of truth for the password policy used by the registration demo.
/// </summary>
public static class PasswordRules
{
    /// <summary>Minimum number of characters a password must contain.</summary>
    public const int MinimumLength = 8;

    /// <summary>Gets the character classes a password has to mix.</summary>
    public static IReadOnlyList<string> RequiredClasses { get; } =
    [
        "upper-case letter",
        "lower-case letter",
        "digit",
        "symbol",
    ];

    /// <summary>Returns <see langword="true"/> when <paramref name="password"/> is long enough.</summary>
    public static bool HasMinimumLength(string? password) => (password?.Length ?? 0) >= MinimumLength;

    /// <summary>Returns <see langword="true"/> when <paramref name="password"/> mixes every required character class.</summary>
    public static bool HasRequiredComplexity(string? password)
    {
        if(string.IsNullOrEmpty(password))
        {
            return false;
        }

        var hasUpper = false;
        var hasLower = false;
        var hasDigit = false;
        var hasSymbol = false;

        foreach(var character in password)
        {
            if(char.IsUpper(character))
            {
                hasUpper = true;
            }
            else if(char.IsLower(character))
            {
                hasLower = true;
            }
            else if(char.IsDigit(character))
            {
                hasDigit = true;
            }
            else if(!char.IsWhiteSpace(character))
            {
                hasSymbol = true;
            }
        }

        return hasUpper && hasLower && hasDigit && hasSymbol;
    }

    /// <summary>Returns <see langword="true"/> when every rule is satisfied.</summary>
    public static bool IsValid(string? password) => HasMinimumLength(password) && HasRequiredComplexity(password);
}
