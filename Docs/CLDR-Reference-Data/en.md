# Unicode CLDR reference data — planned integration

**Status:** upstream files vendored; runtime evaluation, normalization, import and automated testing remain future work.

## Purpose and current baseline

Afutrans stores pinned [Unicode CLDR JSON 48.2.3](https://github.com/unicode-org/cldr-json/releases/tag/48.2.3) supplemental resources under [`Afutrans.Core/Jsons/Cldr/`](../../Afutrans.Core/Jsons/Cldr/), with exact filenames, release identifiers, Git blob SHAs and the full Unicode-3.0 license. This preserves a reproducible **offline source** for future locale features.

This change does not modify existing `Afutrans.Core/Jsons/languages.json` or the runtime `Afutrans.Core.Localization.Localizer`; it does not create `CountryLanguages` rows, an EF Core migration, plural evaluation, or translated grammar variant files.

## Cross-project design contract

The canonical *future* grammar and hint architecture is captured in [UnderstandMe's Layered Localization, Hints and Grammar Variants](https://github.com/afrowaveltd/Toolbox/blob/master/UnderstandMe/Docs/Grammar-Hints-and-Variants/en.md). Basic **dictionary-less startup** and flat JSON localizers must work even with no CLDR data or grammar sidecars. The old localization indexer remains unchanged; a future optional overload may accept composable typed hints before the positional arguments. Afutrans acts as a future generation/import/editor host, while UnderstandMe owns reusable runtime contracts. **None of these runtime features is implemented by bundling the reference data.**

## Sources and intended use

- Cardinal, ordinal and range plural rules: `plurals.json`, `ordinals.json`, `pluralRanges.json`. CLDR expressions contain operands such as `n`, `i`, `v`, `f` and example annotations after `@`; an interpreter or validated rule compiler is still required.
- Grammar data: `gender.json`, `grammaticalFeatures.json`. These describe only the features actually covered by CLDR and do **not** amount to an automatic morphological analyzer.
- Country and language selection: `territoryInfo.json`, `territoryContainment.json`, `languageData.json`. Use for optional default language suggestions, regional filters, and future many-to-many `CountryLanguages` with provenance.
- BCP 47 locale assistance: `aliases.json`, `likelySubtags.json`, `parentLocales.json`. Plan deterministic normalization and full fallback to general language and configured default; do not conflate likely-subtag expansion with dictionary fallback.

## Proposed integration sequence (not implemented)

1. Define a small typed reader and version/manifest validator, using `System.Text.Json`; retain the exact original files as the import source.
2. Evaluate cardinal rules for representative values (for example `cs`: `1`, `2`, `5`, `1.5` and `1.0`) and compare outputs to CLDR category rules. Preserve decimal visible-digit information.
3. Keep **plain `en.json` / `cs.json`** and dictionary-less startup fully functional when no grammar files, no hints or no CLDR component is installed.
4. Represent optional plural/gender/condition variants in **sidecar language files or separate SQL tables**, with manually editable overrides and revision history. Inline hints may request generation of missing variants, but an unverified automatic suggestion cannot erase a manually corrected value.
5. Import language codes including three-letter and BCP 47 variants, and optionally derive country-language associations via EF Core. Treat CLDR coverage as suggestions, not exhaustive membership or language availability.
6. Add compatibility tests for flat-dictionary consumers, missing sidecars, locale fallback, typical and edge-case cardinal categories, and offline operation. No current green-test claim is made for these future modules.

## Licensing, version and maintenance

The bundled files are from upstream release `48.2.3` at `91c267402229a59e3ef2774544f001bf959e8809`. See the [file manifest](../../Afutrans.Core/Jsons/Cldr/manifest.json) and [Unicode-3.0 license](../../Afutrans.Core/Jsons/Cldr/LICENSE). Keep provenance and license on every update. Existing user-authored translations and metadata must be preserved during later reference-data refreshes.

The independently reusable UnderstandMe library should expose relevant locale/grammar contracts; Afutrans is a host and potential importer/editor, not a required runtime dependency for ordinary dictionary lookup.

## More CLDR assets saved for later work (2026-10-19)

Added **19 official JSON files**, pinned to CLDR 48.2.3: eight localized language-name catalogs (`en`, `cs`, `sw`, `lg`, `rw`, `fr`, `ar`, `ha`); four localized territory-name catalogs (`en`, `cs`, `sw`, `lg`); scripts (`en`, `cs`); orientation (`en`, `ar`, `he`); plus `numberingSystems.json` and `weekData.json`. See the [additional manifest](../../Afutrans.Core/Jsons/Cldr/manifest-additional.json) with upstream Git blob SHAs and [README](../../Afutrans.Core/Jsons/Cldr/README.md).

They supplement—but do not replace—the existing `languages.json` with manually useful native names, and do not provide a completed locale provider or complete endonym coverage. No runtime or project build/publish configuration was changed. Their upstream source/Unicode-3.0 license and version are preserved for future UnderstandMe reuse.

