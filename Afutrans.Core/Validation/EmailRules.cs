using System.Net.Mail;

namespace Afutrans.Core.Validation;

/// <summary>Single source of truth for the login (e-mail) rule of the registration demo.</summary>
public static class EmailRules
{
    /// <summary>Returns <see langword="true"/> when <paramref name="value"/> is a syntactically valid e-mail address.</summary>
    public static bool IsValid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && MailAddress.TryCreate(value.Trim(), out _);
}
