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

## 2026-10-19 — configuration sources and independent target polling (design)

- The canonical UnderstandMe design now explicitly supports host-provided settings via `appsettings.json`/IConfiguration, constructor/DI typed options or a custom configuration source. Provider choice, dictionary/metadata accessors, optional notifications/transports and per-target scheduled discovery can be configured independently.
- A target may never emit change events. Afutrans can use a configured **scheduled target scanner** to inspect SQL, API or file-backed dictionaries, compare revisions/snapshots and submit only relevant source changes through normal orchestration.
- Polling complements REST, SignalR or local change events, but requires neither. Push and pull must be deduplicated and share one idempotent job admission path; a translated target update must not create a translation loop.
- Preserve existing planned Scheduler policy: EF Core persistent `NextRunAtUtc`, configurable startup delay, interval after completion, non-overlap of the same task and durable handoff of work rather than waiting for the translation to finish.
- [Canonical configuration and scanning design](https://github.com/afrowaveltd/Toolbox/blob/master/UnderstandMe/Docs/Localization/Configuration-and-Change-Detection/en.md). Documentation only: no scheduled scanner, provider or host API was implemented; the manually authored WebTranslator remains untouched.

## 2026-10-19 — source snapshots, key modes and obsolete phrases

- Each configured Translation Target has an independent source/default culture (e.g. `en`, `cs`) and key convention. `SourceText` dictionaryless registration requires `key=value` in that target's source language; `Symbolic` keys may differ from the original source value.
- The future scanner compares a **durably saved last successfully reconciled source state** with the current authoritative snapshot. Additions, deletions, source-value changes under stable symbolic keys, and relevant metadata revisions are distinct classifications. Stale translations must be reviewed/retranslated when source values change.
- A small JSON target can retain a full previous dictionary copy; a large SQL/API target can use equivalent revisions/ETags/change feeds and persisted key/value hashes. Only checkpoint after durable work handoff, and deduplicate push+poll work.
- A dictionaryless catalog retains old discovered phrases even when the application stops referring to them; a source-dictionary diff alone **cannot prove key disuse**. Optional active-release key manifests and aggregated per-key usage allow stronger evidence; rare screens, offline or older deployed clients must be respected.
- Proposed per-target reversible `Active -> SuspectedUnused -> Archived` policy. Physical deletion remains separately authorized and disabled by default. No translation/metadata history or manual work is destroyed based solely on an inactivity timer.
- [Canonical planned design](https://github.com/afrowaveltd/Toolbox/blob/master/UnderstandMe/Docs/Localization/Dictionary-Snapshots-and-Key-Lifecycle/en.md). This is documentation-only; Scheduler, scanning, key lifecycle and automatic translation are **not** yet implemented in Afutrans. User-authored WebTranslator source remains untouched.

## 2026-10-19 — additional offline CLDR reference data

- Added **19 unmodified JSON reference files** from pinned Unicode CLDR 48.2.3 in `Afutrans.Core/Jsons/Cldr/LocalNames`, `Layout`, `numberingSystems.json` and `weekData.json`.
- Localized language names: `en`, `cs`, `sw`, `lg`, `rw`, `fr`, `ar`, `ha`; localized territory names: `en`, `cs`, `sw`, `lg`; script names: `en`, `cs`; layout: `en`, `ar`, `he`. Additional number system and week-convention reference data are retained.
- Added `manifest-additional.json` with source path, pinned release/commit, Git blob SHA, and Unicode-3.0 license reference; updated CLDR README and `Docs/CLDR-Reference-Data/en.md`.
- No existing `languages.json`, original CLDR assets or application source was changed. **No new runtime, import or language-rule implementation is implied; these are stored reusable datasets only.** Future shared UnderstandMe module/provider still pending.

