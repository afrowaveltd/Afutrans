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
