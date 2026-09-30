using System.Globalization;
using Afutrans.Core.Localization;
using Xunit;

namespace Afutrans.Core.Tests;

public class LocalizerTests
{
    [Fact]
    public void Default_culture_is_english()
    {
        var localizer = Localizer.CreateDefault();

        Assert.Equal("en", localizer.Culture.Name);
        Assert.Equal("Afutrans Translator", localizer[StringKeys.AppTitle]);
    }

    [Fact]
    public void Unknown_key_falls_back_to_the_key_itself()
    {
        var localizer = Localizer.CreateDefault();

        Assert.Equal("Does.Not.Exist", localizer["Does.Not.Exist"]);
    }

    [Fact]
    public void Missing_translation_falls_back_to_english()
    {
        var catalogs = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            [Localizer.FallbackCultureName] = new Dictionary<string, string> { ["K"] = "fallback" },
            ["cs"] = new Dictionary<string, string>(),
        };

        var localizer = new Localizer(catalogs, CultureInfo.GetCultureInfo("cs"));

        Assert.Equal("fallback", localizer["K"]);
    }

    [Fact]
    public void SetCulture_uses_the_catalog_and_raises_the_event_once()
    {
        var catalogs = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            [Localizer.FallbackCultureName] = new Dictionary<string, string> { ["K"] = "hello" },
            ["cs"] = new Dictionary<string, string> { ["K"] = "ahoj" },
        };

        var localizer = new Localizer(catalogs);
        var raised = 0;
        localizer.CultureChanged += (_, _) => raised++;

        localizer.SetCulture(CultureInfo.GetCultureInfo("cs"));

        Assert.Equal("ahoj", localizer["K"]);
        Assert.Equal(1, raised);

        // Selecting the same culture again must not raise anything.
        localizer.SetCulture(CultureInfo.GetCultureInfo("cs"));
        Assert.Equal(1, raised);
    }

    [Fact]
    public void Format_applies_arguments()
    {
        var localizer = Localizer.CreateDefault();

        Assert.Equal("Hello, Ada!", localizer.Format(StringKeys.HelloNamed, "Ada"));
    }
}
