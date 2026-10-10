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

## Next continuation

1. Agree on initial cross-project translation request/result, target and provider contracts against current UnderstandMe documentation; do not treat legacy Dita DTOs as universal provider contracts.
2. Resolve .NET 10/11 package/reference boundaries and the Essentials/WhenItFails dependency versions before integration.
3. Define typed ScheduledTask/TaskExecution and EF Core store/claim semantics, scan handoff acknowledgement, and WorkLedger/WorkReport-to-Logger correlation without introducing unnecessary scheduling priority or missed-run policies.
4. Build a small, end-to-end vertical slice with a test provider before adding actual optional LibreTranslate integration and expanding the Afutrans hosts.
5. Keep root `README.md`, topic-specific English documentation, tests and this status file synchronized and commit each incremental change.
