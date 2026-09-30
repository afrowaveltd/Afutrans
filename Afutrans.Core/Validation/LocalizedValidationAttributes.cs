using System.ComponentModel.DataAnnotations;
using Afutrans.Core.Localization;

namespace Afutrans.Core.Validation;

/// <summary>
/// Ambient localizer used by validation attributes.
/// </summary>
/// <remarks>
/// Data annotations do not take part in constructor injection, so the attributes read the
/// localizer from this fallback (or from the <see cref="ValidationContext"/> when one is
/// available). <c>AddAfutransCore()</c> points it at the DI registered localizer, which keeps a
/// single source of truth for the UI language.
/// </remarks>
public static class ValidationLocalizer
{
    /// <summary>Gets or sets the localizer used by validation attributes.</summary>
    public static ILocalizer Current { get; set; } = Localizer.CreateDefault();
}

/// <summary>
/// Base class for data annotation attributes whose message is a localization key.
/// </summary>
public abstract class LocalizedValidationAttribute(string messageKey) : ValidationAttribute
{
    /// <summary>Gets the localization key of the failure message.</summary>
    public string MessageKey { get; } = messageKey;

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
        IsPropertyValid(value, validationContext)
            ? ValidationResult.Success
            : new ValidationResult(Translate(validationContext, FailureKey(value, validationContext), FailureArgs(validationContext)));

    /// <summary>Gets the localization key of the message to report for the current <paramref name="value"/>.</summary>
    protected virtual string FailureKey(object? value, ValidationContext validationContext) => MessageKey;

    /// <summary>Gets the arguments used to format the failure message.</summary>
    protected virtual object?[] FailureArgs(ValidationContext validationContext) => [];

    /// <summary>Returns <see langword="true"/> when <paramref name="value"/> satisfies the rule.</summary>
    protected abstract bool IsPropertyValid(object? value, ValidationContext validationContext);

    /// <summary>Resolves a localization key through the context service (or the ambient localizer).</summary>
    protected static string Translate(ValidationContext context, string key, params object?[] args)
    {
        var localizer = context.GetService(typeof(ILocalizer)) as ILocalizer ?? ValidationLocalizer.Current;

        return localizer.Format(key, args);
    }
}

/// <summary>Requires a non-empty value, with a localized message.</summary>
public sealed class RequiredLocalizedAttribute(string messageKey) : LocalizedValidationAttribute(messageKey)
{
    /// <inheritdoc />
    protected override bool IsPropertyValid(object? value, ValidationContext validationContext) =>
        value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
}

/// <summary>Requires a syntactically valid e-mail address, with a localized message.</summary>
public sealed class EmailLocalizedAttribute(string messageKey) : LocalizedValidationAttribute(messageKey)
{
    /// <inheritdoc />
    protected override bool IsPropertyValid(object? value, ValidationContext validationContext) =>
        EmailRules.IsValid(value as string);
}

/// <summary>Enforces <see cref="PasswordRules"/> (length and character classes) with localized messages.</summary>
public sealed class PasswordRulesAttribute(string lengthMessageKey, string complexityMessageKey, int minimumLength = PasswordRules.MinimumLength)
    : LocalizedValidationAttribute(lengthMessageKey)
{
    /// <summary>Gets the localization key of the "too short" message.</summary>
    public string ComplexityMessageKey { get; } = complexityMessageKey;

    /// <summary>Gets the required minimum length.</summary>
    public int MinimumLength { get; } = minimumLength;

    /// <inheritdoc />
    protected override bool IsPropertyValid(object? value, ValidationContext validationContext)
    {
        var password = value as string;

        return PasswordRules.HasMinimumLength(password)
               && password!.Length >= MinimumLength
               && PasswordRules.HasRequiredComplexity(password);
    }

    /// <inheritdoc />
    protected override string FailureKey(object? value, ValidationContext validationContext) =>
        PasswordRules.HasMinimumLength(value as string) ? ComplexityMessageKey : MessageKey;

    /// <inheritdoc />
    protected override object?[] FailureArgs(ValidationContext validationContext) => [MinimumLength];
}

/// <summary>Requires this value to equal another property of the same instance (password confirmation).</summary>
public sealed class PasswordConfirmationAttribute(string otherPropertyName, string messageKey) : LocalizedValidationAttribute(messageKey)
{
    /// <summary>Gets the name of the property that has to match.</summary>
    public string OtherPropertyName { get; } = otherPropertyName;

    /// <inheritdoc />
    protected override bool IsPropertyValid(object? value, ValidationContext validationContext)
    {
        var other = validationContext.ObjectInstance
            .GetType()
            .GetProperty(OtherPropertyName)?
            .GetValue(validationContext.ObjectInstance);

        return string.Equals(value as string, other as string, StringComparison.Ordinal);
    }
}
