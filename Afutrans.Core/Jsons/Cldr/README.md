# Unicode CLDR reference data for Afutrans

**Status: source data bundled for future implementation; no CLDR runtime parser, database importer or grammar engine is included by this change.**

This directory carries unmodified **Unicode CLDR JSON 48.2.3** supplemental files, pinned to the upstream release of **2026-09-30**. These are **not** Afutrans translation dictionaries. The existing sibling `../languages.json` is left untouched.

- Upstream: [unicode-org/cldr-json](https://github.com/unicode-org/cldr-json)
- Release: [48.2.3](https://github.com/unicode-org/cldr-json/releases/tag/48.2.3)
- Source revision: `91c267402229a59e3ef2774544f001bf959e8809`
- Upstream location: `cldr-json/cldr-core/supplemental/`
- Licensing: **Unicode-3.0**; see the full copied [LICENSE](LICENSE).
- File inventory and upstream blob identifiers: [manifest.json](manifest.json).

## Included datasets

| File | Intended future use |
| --- | --- |
| [plurals.json](plurals.json) | Cardinal plural categories by language, such as `one`, `few`, `many`, `other` |
| [ordinals.json](ordinals.json) | Ordinal number categories |
| [pluralRanges.json](pluralRanges.json) | Plural categories for numeric ranges |
| [gender.json](gender.json) | CLDR's limited gender information (not a universal gender inflector) |
| [grammaticalFeatures.json](grammaticalFeatures.json) | Available grammatical feature metadata for supported locales |
| [territoryInfo.json](territoryInfo.json) | Country-language association, population estimates, official status |
| [territoryContainment.json](territoryContainment.json) | Geographical region hierarchy (e.g., Africa / East Africa) |
| [languageData.json](languageData.json) | Language-to-script metadata and related language information |
| [likelySubtags.json](likelySubtags.json) | Likely language/script/region expansions |
| [parentLocales.json](parentLocales.json) | Explicit locale parent relationships and special inheritance |
| [aliases.json](aliases.json) | Legacy/alternate language, script, territory and other identifiers |

## Architectural boundaries

1. **Dictionary-less localization remains available without these files.** Basic flat locale dictionaries must work on their own.
2. Optional grammar/placeholder metadata belongs in separate files/tables, never embedded as nested objects inside basic flat language dictionaries.
3. A future CLDR adapter can load, validate and compile plural expressions offline. **These JSON expressions are data, not executable C# or SQL.**
4. Import into EF Core `Languages` / `Countries` / `CountryLanguages` is **not yet implemented**. Existing `languages.json` may be expanded later using three-letter BCP 47 language tags and regional variants.
5. CLDR territory speaker data are approximate and incomplete for minority languages. Do not infer that a language is forbidden merely because it is absent.
6. Grammar selection depends on runtime arguments and separately authored or verified translated variants. These datasets do not provide translated sentence forms or generate full case, tense, gender and noun-class inflections.
7. The source release is pinned for reproducibility. Updates require explicit review of upstream changes, tests and license attribution; manual user-edited grammar variants must not be overwritten.
8. The data are source-controlled **references**. No new project build-copy or runtime initialization behavior has been added.

For the development plan and planned integration boundaries, see [the English CLDR data guide](../../../Docs/CLDR-Reference-Data/en.md).

## Additional CLDR 48.2.3 offline assets (2026-10-19)

Additional **19 original JSON files** are vendored, preserving exact Unicode CLDR source contents. [The additional manifest](manifest-additional.json) records each official upstream path and Git blob SHA. The existing [Unicode-3.0 license](LICENSE) applies to these files, and the original [manifest.json](manifest.json) remains unchanged.

| Directory/file | Data available | Future possible usage |
| --- | --- | --- |
| `LocalNames/{en,cs,sw,lg,rw,fr,ar,ha}/languages.json` | Names of languages translated *into the named locale* | Menus with localized names, expanded language catalogue and endonym lookup when each language's own locale data exists |
| `LocalNames/{en,cs,sw,lg}/territories.json` | Translated country/region names | Country picker and regional filters |
| `LocalNames/{en,cs}/scripts.json` | Translated script names | BCP 47 script selectors and multilingual language menu |
| `Layout/{en,ar,he}/layout.json` | Character and line orientation per locale | Sample RTL/LTR layout information; actual mixed-script direction still needs locale/script-aware resolution |
| `numberingSystems.json` | Decimal digits and other numbering systems | Formatting language-specific numerals |
| `weekData.json` | First day of week/weekend and related CLDR territory conventions | Calendar and locale-sensitive scheduling UI |

**Read carefully:** This is *reference data only*. CLDR `languages.json` files in `LocalNames/` are localized display names and do **not** replace `../languages.json` or automatically represent all endonyms. `territoryInfo.json` separately maps languages to countries and is incomplete for some minorities/migrants. These datasets do not imply that an external translator supports every language, nor do they add a runtime CLDR parser or a new API.

The future canonical shared catalog/provider should live in Toolbox UnderstandMe as an optional component. Move or deduplicate these pinned assets **only after** a tested provider can consume them without losing manually curated names and metadata. Do not make base localization require CLDR.

