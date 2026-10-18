# Afutrans implementation status

Last updated: **2026-10-11**  
Branch: `main`  
Phase: **UI/application foundation implemented; translation product architecture and integration remain design-stage**.

This file is the continuation point for the Afutrans project. Update it after each repository change, distinguishing implemented behavior from planned behavior and recording actual verification results.

## Verified repository baseline

- Repository: [afrowaveltd/Afutrans](https://github.com/afrowaveltd/Afutrans); solution: `Afutrans.slnx`.
- Projects present: `Afutrans.Core`, `Translator` (Avalonia), `TUI.Translator` (Terminal.Gui), `WebTranslator` (ASP.NET Core), `Afutrans.Core.Tests`, and `TUI.Translator/Tests`.
- Existing shared UI foundation: dependency injection, localization catalog, shared view models and navigation, the `Main`/`Setup`/`Test` demonstration pages, and validation tests.
- WebTranslator already contains Serilog setup, a JSON daily application log sink, a SQLite warning/error sink, retention/cleanup services, and SignalR-related application logging infrastructure.
- Current Afutrans.Core and WebTranslator projects target `net11.0`. The shared UnderstandMe library in Toolbox is an unimplemented `net10.0` design skeleton; Afutrans is not yet integrated with a functioning UnderstandMe translation engine.
- The existing `IMPLEMENTATION_PLAN.md` records the prior UI-foundation work package and historical build/test evidence. It is not proof of a fresh test run today.
- **Not implemented/verified:** production translation orchestration, multi-provider workers, QueueMaster, schedule/scan persistence, dictionary syncing, assisted document editor, WorkLedger, and full reports. Early source/backend enums or models are not operational connectors.

## 2026-10-11 — product README rewrite (documentation only)

- Replaced the old demo-only root `README.md` description with an English, user-oriented product overview explaining what Afutrans is intended to do.
- Documented intended scenarios: localization dictionaries, document and dynamic text translation, passive/active translation targets, scheduled scanning, optional machine/AI providers, human work, editor/revisions, resource and access policies, privacy, and monitoring.
- Captured current scheduling design: database-backed EF Core, stored `NextRunAtUtc`, delay after startup, interval measured from run completion, concurrent different tasks without overlapping the same task, and no scheduler-owned priority decisions.
- Documented the separation between the existing **Application Logger** and future **WorkLedger/WorkReport**, with optional stable links from work events to technical log events and separate retention.
- Added a truthful present-vs-planned feature/status table, current foundation build/run instructions, and links to the detailed UnderstandMe design.
- No code, NuGet references, project frameworks, APIs, or runtime behavior changed in this documentation pass.

## Verification

- Inspected current GitHub `main` file tree, project files, previous README and implementation plan, and UnderstandMe design/integration documentation.
- Only Markdown documentation changed. **No build/test suite was run in this pass**, so no new green-test claim is made.
- Markdown links target known files/directories in the current repository or explicit external GitHub source documents.

## 2026-10-11 — bundled Unicode CLDR 48.2.3 reference data

- Added **11 upstream, unmodified supplemental CLDR JSON data files** under `Afutrans.Core/Jsons/Cldr/`: cardinal plurals, ordinals, plural ranges, gender, grammatical features, territory info, territory containment, language data, likely subtags, parent locales and aliases.
- Pinned the upstream release `48.2.3` (published 2026-09-30), source commit `91c267402229a59e3ef2774544f001bf959e8809`, upstream blob identifiers in `manifest.json`, and the full Unicode-3.0 `LICENSE`.
- Added local CLDR README and `Docs/CLDR-Reference-Data/en.md`, and linked the new reference assets from the root README.
- Preserved the original `Afutrans.Core/Jsons/languages.json` exactly; no project files, executable code, runtime EF Core integration, rules interpreter or locale fallback code changed.
- The datasets were parsed as JSON at preparation time and checked for CLDR major version 48, cardinal rules for `cs`/`en`/`sw`/`ar`, Uganda language associations, Africa region containment and locale parent metadata.
- **Validation scope:** data-source integrity and GitHub file checks only. No `dotnet build` or `dotnet test` was run; no runtime capability is claimed.

## 2026-10-11 — linked canonical UnderstandMe grammar/hint design (documentation only)

- Linked `Docs/CLDR-Reference-Data/en.md` to the now-committed UnderstandMe [layered localization, hints and grammar design](https://github.com/afrowaveltd/Toolbox/blob/master/UnderstandMe/Docs/Grammar-Hints-and-Variants/en.md) in Toolbox `master`.
- The reference data remains pinned and untouched; the future Afutrans importer/editor must preserve per-locale manual overrides and dictionary-less runtime fallback.
- **No code, runtime behavior, NuGet references, project files or CLDR datasets changed. No build/test run.**

## Next continuation

1. Agree on initial cross-project translation request/result, target and provider contracts against current UnderstandMe documentation; do not treat legacy Dita DTOs as universal provider contracts.
2. Resolve .NET 10/11 package/reference boundaries and the Essentials/WhenItFails dependency versions before integration.
3. Define typed ScheduledTask/TaskExecution and EF Core store/claim semantics, scan handoff acknowledgement, and WorkLedger/WorkReport-to-Logger correlation without introducing unnecessary scheduling priority or missed-run policies.
4. Implement an optional, separately tested offline CLDR data reader/importer and plural-rule resolver **without coupling basic dictionary-less lookup to CLDR**. Then build a small, end-to-end vertical slice with a test provider before adding actual optional LibreTranslate integration and expanding the Afutrans hosts.
5. Keep root `README.md`, topic-specific English documentation, tests and this status file synchronized and commit each incremental change.


## 2026-10-18 — UnderstandMe localization baseline and integration boundary

- [UnderstandMe in Toolbox](https://github.com/afrowaveltd/Toolbox/tree/master/UnderstandMe/Localization) now contains an independent .NET 10 flat-JSON localizer with source-key discovery, culture fallbacks, basic named placeholders, explicit reload and a pluggable store.
- The new UnderstandMe test project has **14/14 passed tests** in the [verified Toolbox CI run](https://github.com/afrowaveltd/Toolbox/actions/runs/39167610994). The entire Toolbox solution reported 5,928 passed tests and zero build warnings.
- **Afutrans has not yet been integrated or tested against UnderstandMe.** No new package reference, migration, UI behavior or automatic translation integration should be claimed.
- The existing Afutrans.Core `ILocalizer`, `Localizer`, `StringKeys` and built-in catalogs are generated/provisional UI scaffolding. They may be refactored or replaced as needed to implement the agreed design. Crucially, they currently use symbolic keys (e.g. `App.Title`), so any switch to source-text `key=value` dictionaryless behavior must explicitly supply/migrate source phrases rather than displaying symbolic keys.
- **Do not rewrite the manually started user-authored WebTranslator project.** Avoid breaking its build and published host contract while making core/library changes; dedicated web business REST and SignalR integration remains a future, separately reviewed step.
- Dita is completely separate and inspiration-only: no code or runtime dependency may be copied into UnderstandMe or Afutrans.
- Next Afutrans work: once an UnderstandMe project/package dependency is reliably consumable, add an adapter against its stable localization interface, rework generated GUI/TUI scaffolds as desired, migrate source catalogs to shared flat JSON and run **Afutrans.Core.Tests and GUI/TUI build validation**. Only then extend placeholder metadata, hot reload, or background translation testing.

Documentation-only Afutrans change; the web application and all source code remain untouched.
