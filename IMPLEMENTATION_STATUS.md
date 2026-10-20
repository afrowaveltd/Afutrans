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

## 2026-10-20 — Afutrans as optional registered Afrowave service application (design)

- Product direction: Afutrans is a registered **service application** in Id (one among many), used by Id itself and optionally exposed to other registered applications as a public/listed service according to configured service and client access policies. Directory visibility != authorization or entitlement.
- Consumer application owners configure independent Translation Targets (local file roots, SFTP/FTPS, SQL/DataAccess or API/SignalR) with their own source/target languages, access rights, secrets references, quotas, sync settings and privacy boundaries. Id owns app registry/grants, **not** raw target DB connection strings or dictionary content.
- Application-owned first-party cookie sessions plus an Id-only central authentication session replace any idea of routing/sharing one cookie across apps; no third-party identity provider sign-in/tracking cookies, and no hard dependency on Afutrans for Id bootstrap/login.
- Added [Docs/Registered-Service-Integration/en.md](Docs/Registered-Service-Integration/en.md) and linked it from the root README. No runtime code, migration, connector, API or tests changed. **Do not present these capabilities as implemented.**

## 2026-10-20 — connector freedom and community dictionary opt-in (design)

- Clarified user product policy: application owners may intentionally use classic FTP or test HTTP, with visible risks, instead of a platform-wide prohibition. Host-side network/credential/parser/tenant isolation remains mandatory; Id's own sign-in and token transport cannot be degraded by that choice.
- Afutrans must disclose that it technically reads authorized target dictionaries to process translations. Separate `Private` (default), selected-entry sharing and owner-approved eligible whole-dictionary publication with publication preview, provenance, downstream redistribution rights and withdrawal limitations.
- Common shared translation memory should reuse compatible translations across contributing apps but protect owner overrides, source/target locale, context, placeholders and publication scope; private source, message contents and credentials never enter public shared indexes implicitly.
- Added [Docs/Connector-Trust-and-Community-Dictionaries/en.md](Docs/Connector-Trust-and-Community-Dictionaries/en.md), aligned Registered Service Integration and README. **Documentation only: no connector, sharing engine, API, migrations or tests added.**

## 2026-10-20 — planned service intermediary and open/authorized modes

- Owner proposes a separate reusable intermediary/API gateway so hosted Afutrans may remain private, exposing only permitted operations through a public edge. The intermediary can validate Id-backed user/application grants, distinguish anonymous open request mode from authorized mode and route allowed requests.
- Allow anonymous translation only when explicitly configured and quota/privacy/resource constrained. Creating/updating Translation Targets, SQL/FTP connectors, secrets or private dictionaries remains authorized even in an otherwise open translation deployment.
- Retain mandatory target-level capability and caller-context verification at Afutrans's application-command boundary to prevent bypass; no trusted user-provided IDs or insecure forwarding headers. UnderstandMe remains independent, offline-capable, and does not require an Id SDK.
- Recorded in [Docs/Registered-Service-Integration/en.md](Docs/Registered-Service-Integration/en.md). **Design only: user intentionally deferred detailed API/role/gateway implementation; no runtime changes or tests performed.**

## 2026-10-20 — community-free deferred work and contributor-funded capacity (design)

- Author proposes a sustainability model for **hosted Afutrans**: free accepted deferred translations (finite bounded queues/caps, eventual work under fair capacity allocation), optional contributor-funded priority and optionally dedicated LibreTranslate VPS/worker pool funded by subscription/contributions. Not a profit-first or mandatory paid product; operator may sponsor services.
- Keep `P0 RealTime` genuinely volatile, instant-path, automatic-only. A free waitable request is a separate authorized durable **non-P0** job; plan entitlement constrains admission/worker allocation, rather than redefining QueueMaster P0–P4 or adding mandatory per-client physical queues.
- Use existing `ClientContext`/`ClientPolicy`, Gateway/Id optional authorization, quota/account constraints, QueueMaster and provider-cost separation. Protect internal workloads and give free admitted jobs starvation bounds; distinguish donations/revenue, provider charges and VPS operating costs.
- Added [Docs/Community-Capacity-and-Contributor-Priority/en.md](Docs/Community-Capacity-and-Contributor-Priority/en.md) and linked from product README and service integration. **No payment integration, new gateway, scheduler/workers, code changes or tests.** Exact economics/service guarantees intentionally open.

## 2026-10-20 — corrected isolation policy: dedicated queues, BYO providers and cost dashboard

- **Supersedes preceding contributor-priority proposal:** commercial priority must NOT preempt or receive preferential service from the free community's reserved workers. Maintain isolated `AfrowaveInternal` (including first-party P0 RealTime), `CommunityFree` (explicit durable deferred P1–P4) and dynamically registered per-tenant queue/worker domains. Payment buys independent resources, not queue jumping.
- Tenant may fund managed LibreTranslate VPS or supply existing compatible LibreTranslate, Google/Microsoft cloud account, Ollama/AI or provider endpoint; ownership-specific credentials/costs/allowed backend routing never silently borrow our infrastructure. No mandatory VPS subscription for a self-funded backend.
- Multi-lane QueueMaster design: retain P0–P4 *inside* each lane, bounded resource pools, durable restart-safe lane lifecycle, authentication, per-tenant isolation, no unapproved cross-lane fallback. Customer hardware has no artificial word quotas, but finite CPU/RAM/concurrency, fair abuse/resource protection remain.
- Proposed honest publicly visible contribution/cost breakdown showing gross receipts, VPS/network/backups/fees/taxes/support when relevant, worker capacity funded, and funding for public community capacity; no invented dollar amounts or unrealistic unlimited performance claims.
- Added [Docs/Dedicated-Capacity-BYO-Backends-and-Transparency/en.md](Docs/Dedicated-Capacity-BYO-Backends-and-Transparency/en.md); updated previous community-funding doc with explicit supersession and root README/service-integration design. **Docs only: no scheduler/provider/payment UI or new tests implemented.**

## 2026-10-20 — adaptive waiting, voluntary shared buffers, external agents (design)

- CommunityFree should prefer **slower deferred completion over exhausting arbitrary per-user monthly request/word credits**. Real compute, admission, durable-storage, concurrency, abuse protections and paid-provider hard budgets remain enforceable; never accept unfinishable work or silently persist P0.
- Separate internal/community/tenant QueueLanes remain isolated but **explicit owner-authorized, temporary, revocable, non-guaranteed loans of available endpoint buffer slots** can increase capacity. Internal realtime stays protected; when internal demand returns, stop new borrowed work and safely drain/finish bounded in-flight work (no imaginary immediate preemption).
- Owner may voluntarily share an AI or translation backend with others while borrowing authorized LibreTranslate resources in return; sharing is optional per direction, not compulsory barter, and requires provider-owner consent, caller/target processing privacy policy, billing caps and endpoint/worker isolation. Customer may route own realtime to own engine and other approved deferred work elsewhere.
- For private customer-hosted engines, optional authenticated outbound agent is an alternative to exposing public inbound ports. User's ~$6/month VPS estimate is illustrative and needs workload-specific LibreTranslate benchmarking before advertising.
- Added [Docs/Cooperative-Backend-Sharing/en.md](Docs/Cooperative-Backend-Sharing/en.md); updated capacity model, READMEs and [Toolbox QueueMaster proposal](https://github.com/afrowaveltd/Toolbox/blob/master/UnderstandMe/Docs/Capacity-Lending-and-Backpressure/en.md). **Documents only; no new feature or tests.**

## 2026-10-20 — non-profit community charter and exact donation-to-cost accounting

- **Confirmed overriding mission:** Afrowave services should give everyone the maximum feasible services **free by default from launch and for the long term**; no personal profit, no deliberate scarcity, no donations buying priority over free users and no arbitrary credits/quotas designed to force payment. Prefer slower durable deferred processing and clear information; only actual capacity/storage/abuse safety guards apply.
- Every voluntary contribution must be assigned solely to actual Afrowave service operations, optionally purpose-bound: e.g. one-time paid AI tokens, recurring VPS/LibreTranslate servers, network/backups/security. **No personal enrichment/hidden commission**, no fees disguised as server spending. Unspent funds remain purpose-bound/openly reported, not falsely recorded as expenses.
- Prepare full **last-cent transparent ledger**: gross receipts, third-party transaction fees, taxes, FX, restricted vs general fund, allocations vs paid invoices, verified provisioned VPS/provider credits, refunds/adjustments and remaining balances. Public privacy-safe cost dashboard + donor-specific reconciliation/receipts, with evidence and audited corrections.
- Added [Docs/Community-First-No-Profit-and-Transparent-Funding/en.md](Docs/Community-First-No-Profit-and-Transparent-Funding/en.md) as governing product charter; older paid-tier/paywall examples expressly subordinate. Updated Afutrans README and associated capacity docs. **Documentation only; no finances, payment, public ledger UI, or tests were implemented.**

## 2026-10-20 — split-purpose contributions, monthly reserves and deferred finance tooling

- Contributors can explicitly **split a single contribution by purpose**, e.g. USD 10 -> USD 5 AI credits + USD 5 VPS operating reserve. Each split remains individually traceable to the payment, subject to an upfront-disclosed fee/FX treatment; never silently divert donor-restricted funds.
- **Carry forward surplus:** excess pooled server contributions are held in their purpose-specific balance for future months, with paid bills, reserved future service periods, actual AI credits and unspent cash clearly distinguished and reconciled.
- **No immediate finance implementation:** the donor page, ledger and financial watchdog are deliberately **deferred until functioning useful services exist and accepting voluntary contributions makes practical sense**. Updated [Docs/Community-First-No-Profit-and-Transparent-Funding/en.md](Docs/Community-First-No-Profit-and-Transparent-Funding/en.md). Documentation only, no collection/payment processing, code or tests.

## 2026-10-20 — simplified donor-facing cost totals, detailed evidence held internally

- User clarified public transparency should be **simple and exact**, not a Czech electricity-bill-style breakdown: gross contribution, real total transfer fee, net received/available, purpose split, actual whole purchase price (tax inclusive), remaining purpose balance and value delivered.
- Do **not** split public VPS expenses into VAT/rates/accounting subcomponents; detailed invoices, tax and FX remain in internal/source evidence for accurate reconciliation and compliance. Show extra *real* transaction fees explicitly in one clear total rather than disguising them.
- Updated [Community-First-No-Profit-and-Transparent-Funding](Docs/Community-First-No-Profit-and-Transparent-Funding/en.md) and README. **Design only; finance watchdog/payment code deferred until useful live services exist; no tests or application changes.**

