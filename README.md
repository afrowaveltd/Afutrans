# Afutrans

**Afutrans is an open, modular translation and localization workspace for people, applications, and services.** Its goal is to make it practical to translate and maintain UI dictionaries, documents, and other text sources; coordinate automatic and human translation; review and correct results; and understand the work performed by the system.

> **Project status: early application foundation + architecture/design.** The repository contains working shared UI infrastructure and experimental host applications. The end-to-end translation engine, scheduled target synchronization, editor, and work tracking described below are **planned, not yet implemented**. This README describes the intended product, with a separate section for the code that exists today.

Afutrans will be a complete *application and service host* built primarily from the reusable [UnderstandMe toolkit](https://github.com/afrowaveltd/Toolbox/tree/master/UnderstandMe) in [Afrowave Toolbox](https://github.com/afrowaveltd/Toolbox). UnderstandMe is intended to remain independently usable; consuming applications should **not** need Afutrans to use its translation or localization APIs.

## What Afutrans is intended to do

### Translate text, documents, and localization data

- Translate individual texts, larger documents, and batches of translation requests.
- Work with plain text, Markdown, HTML, and flat JSON localization dictionaries through format-aware adapters; add further formats and sources through extensions.
- Preserve meaningful document structure, protected placeholders, links, and other significant markup. Report structural problems rather than silently delivering damaged output.
- Support standard .NET localization and **dictionary-less** application localization: a missing source phrase can be registered as a key for later translation.
- Keep application localization (`KEY=VALUE` dictionaries) separate from dynamic text translation. Translating a chat message or arbitrary text must **not** automatically create dictionary entries.
- Import, inspect, compare, edit, validate, export, and synchronize language dictionaries across configured projects and targets.

### Connect applications and translation targets

A **Translation Target** describes what Afutrans should maintain: the source location and format, available languages, translation settings, access capabilities, storage destination, and policies. A target may represent a local folder, file tree, database, application API, or another supported data source.

Afutrans will support both applications that actively request translations and **passive targets** that cannot request work themselves. A scanner can compare their current state with previously confirmed state, discover added/changed/missing entries, and submit the necessary work. Read-only sources must remain valid targets; writing back is permitted only when the connector and policy explicitly support it.

### Registered Afrowave service application (planned)

Afutrans can be registered in Afrowave **Id** as a service application, run translations for Id itself, and optionally offer REST/business SignalR translation services to other registered community applications. A public *listing* or service capability does not automatically grant usage, unlimited capacity or access to another application's dictionaries. Each owning app configures an authorized Translation Target (such as a local folder, SFTP source, SQL/data-access connection or its API), default/target languages, source policy and separate client quota.

Id owns first-party identity, application/service discovery and scoped access grants; Afutrans owns translation targets, provider orchestration and dictionary handling; UnderstandMe remains a reusable offline-capable engine. Login sessions/cookies belong to each application (Id has its own limited login cookie), not a shared tracking router. **No Id connection, network listener, provider or user-tracking integration becomes mandatory.** See [Registered Service Integration](Docs/Registered-Service-Integration/en.md) for boundaries and future workflows.


### Owner-selected transport and shared community translations (planned)

Target owners may explicitly choose classic FTP or non-sensitive HTTP as well as preferred SFTP/FTPS/HTTPS, with transparent security warnings and strong **isolation of the hosted service and all other tenants**. Id's central authentication credentials and redirect flows never inherit a tenant's insecure transport choice.

Afutrans must tell the user it can technically read connected dictionaries for translation. **Private is the default**; owners may voluntarily contribute selected eligible entries or an eligible dictionary into a shared Afrowave translation memory, which lets other authorized applications reuse compatible approved phrases rather than translating identical text repeatedly. Public service access, private content access and community dictionary publication are distinct decisions. See [Connector Trust and Community Dictionaries](Docs/Connector-Trust-and-Community-Dictionaries/en.md).


### Community-funded translation capacity (planned)

An optional hosted Afutrans deployment may offer **free, explicitly deferred translation jobs** while inviting voluntary contributions or verified subscription plans to fund faster capacity or dedicated LibreTranslate workers/VPS. Community access remains bounded by honest queue/rate limits and protected from starvation; contributor priority is not a license to bypass privacy, quotas or the actual P0 real-time semantics. Donations, customer entitlements, provider API expenses and infrastructure costs are accounted for separately. A local/offline/self-hosted UnderstandMe installation never requires billing or an Id account. See [Community Capacity and Contributor Priority](Docs/Community-Capacity-and-Contributor-Priority/en.md).


### Run scheduled background work

The planned **Scheduler** is deliberately simple and separate from translation priorities or provider selection:

- Store individual scheduled tasks and their typed target/operation parameters using **EF Core** persistence and migrations.
- Store the next planned execution time (`NextRunAtUtc`) rather than repeatedly calculating due dates from the last execution.
- Treat an interval as **rest time after a run completes**, successfully or unsuccessfully; the next due time is completion time plus interval.
- Respect a per-task startup delay, so a restart does not immediately activate every overdue target.
- Execute overdue work as soon as eligible, without replaying every missed historical interval.
- Dispatch independently due tasks concurrently, but **never run the same scheduled task twice at once**.
- Sleep until the nearest known due time, and wake when a task or plan is added, changed, or completed. Database state remains authoritative; durable claims/recovery prevent duplicate work and lost handoffs.

A scheduled scanner is considered finished when it has completed its scan and **safely submitted** any resulting translation work. It does not need to wait for a long-running or human translation to finish. The orchestrator and QueueMaster own the later translation lifecycle.

### Coordinate different translation providers

Afutrans will expose configurable translation routes rather than require one engine. Planned provider families include:

- **Human translators**, including manual translation, corrections, review, and a durable inbox of authorized work.
- Optional self-hosted or free engines such as **LibreTranslate** and the separately proposed **AfroLT**.
- Optional commercial engines such as Google or Microsoft Translator.
- Optional AI-based engines and editing assistance, including locally hosted models.

**Human translation is the baseline capability; no machine translation, AI subscription, or paid service is required.** Every automatic provider is opt-in. An installation may be manual-only, local-only, or configured to use multiple backends.

The underlying UnderstandMe orchestrator will select compatible enabled providers according to target and administrator policies, capabilities, quotas, availability, and allowed request preferences. It may retry a failed provider or use an eligible alternative; a request must never bypass privacy restrictions or approved spending limits. The actual provider used and any substitution must be visible.

### Manage translation work reliably

UnderstandMe is planned to own five translation-work priorities through **QueueMaster**, with independent provider/worker buffers, acceptance versus completion states, and appropriate persistence and recovery. The priority of translation work belongs to the target/orchestrator policy, **not** to the Scheduler.

A person can accept and complete durable, eligible translation tasks even if no automatic backend exists or no translator was online when work was submitted. The exceptional **P0 RealTime** path is automatic-only and short-lived: it never silently sends private chat text to a human inbox. With no enabled automatic engine, RealTime returns the source instead of accumulating manual work.

Optional workflow chains can split, translate, validate, review, assemble, and deliver work through explicit, replaceable stages.

### Edit and review translations

The planned assisted editor will offer:

- Side-by-side original and translated content, with a responsive layout for smaller screens.
- Direct editing in rendered text (WYSIWYG) and HTML/Markdown source views; both work on the same draft.
- Segment-level inspection, manual corrections, retry/suggestion actions, and visible validation warnings.
- Safe formatting tools such as bold/italic and guided transfer of source markup to selected translated text **without re-translating that text**.
- Background autosave starting **1,000 ms after the last manual edit**, with real saving/saved/error states.
- Attributable revision history, undo/revert as a new revision, and protection against stale machine/AI output overwriting newer human edits.
- Drafts that remain editable when temporarily invalid, while publishing/export requires appropriate validation.
- Optional, explicitly requested AI assistance whose suggested changes remain reviewable and reversible.

The editor must remain useful with **human translation alone**. Optional AI assistance must never become a prerequisite for basic editing.

### Keep a work record distinct from application logs

Afutrans is intended to have **two complementary records**:

| Record | Purpose |
| --- | --- |
| **Application Logger** | Technical diagnostics, framework and HTTP events, exceptions, security and application failures. |
| **WorkLedger** (*work activity / work record*) | What a scheduled scan, translation, provider, worker, or result-processing operation actually did, with progress, timings, outcomes, and resource/throughput measures. |

A specific execution produces a structured **WorkReport** and related work entries, including operations still in progress. Work activity can count items, processed characters, provider attempts/failures, queue throughput, and completion states.

Where a technical error explains a work outcome, a work entry should **link to the corresponding application log event** using stable identifiers rather than duplicate stack traces or HTTP diagnostics. Correlation can work in both directions; log storage and retention remain independent. Work activity, technical logs, and mandatory manual-edit revision/audit history have **separate maintenance and retention rules**.

The operator UI will make work history, reports, backend behavior, and live progress understandable without searching raw log files.

### Offer configurable access, privacy, and monitoring

Afutrans is intended to run locally or as a persistent multi-client host. Planned capabilities include:

- Configurable targets, authorized source roots, and per-client/provider access policies.
- Optional REST/API integration; **no public translation endpoint is enabled merely by installing the library**.
- Configurable provider quotas, account-level budgets, usage accounting, and spending ceilings.
- Optional local/live monitoring and SignalR event projections, independent of the system that owns durable work.
- Privacy-scoped translation results: a consuming chat/forum application owns its message history, access checks, and optional recipient-specific corrections. Private messages are not shared with a general dictionary or human/audit queue.

## How the pieces fit together

```text
Sources / Applications / API / Manual UI
                  |
           Translation Targets
                  |
     Scheduler (optional) -> Target Scanner
                  |                |
                  +-----> UnderstandMe Orchestrator
                                   |
               TextSource / validation / policies
                                   |
                         QueueMaster + workers
                       /       |          \
                  Human   LibreTranslate  Other optional providers
                       \       |          /
                        ResultProcessor
                              |
                    DataAccess / host results
                              |
                  WorkLedger / WorkReports
                     (links to app logs)

Afutrans Web / Desktop / TUI: configuration, editor, reports and monitoring
```

This is a **target architecture**, not a diagram of currently implemented classes. Core processing belongs in reusable UnderstandMe components; the Afutrans hosts provide presentation, configuration, operator workflows, and application-specific integration.

## Repository structure and current implementation

| Project | Current role | Current status |
| --- | --- | --- |
| [`Afutrans.Core`](Afutrans.Core/) | Shared localization, navigation, view models, validation, and dependency injection; early source/settings models | **Foundation implemented** |
| [`Translator`](Translator/) | Avalonia desktop application | **UI skeleton / demonstration views** |
| [`TUI.Translator`](TUI.Translator/) | Terminal.Gui v2 console application | **UI skeleton / demonstration views** |
| [`WebTranslator`](WebTranslator/) | ASP.NET Core web host, Razor Pages, Serilog-based application logging, and SignalR infrastructure | **Web/application infrastructure skeleton** |
| [`Afutrans.Core.Tests`](Afutrans.Core.Tests/) | Shared core tests | **Existing tests; no new run claimed here** |
| [`TUI.Translator/Tests`](TUI.Translator/Tests/) | Console UI/binding tests | **Existing tests; no new run claimed here** |

The current `Main`, `Setup`, and `Test` pages, `IHelloService`, and registration-form validation are **learning/architecture demonstrations**, not the finished translation workspace. The current source/backend enums and models are preliminary and do not mean that their connectors are implemented. The web logging infrastructure exists, but **WorkLedger is a future component**.

The repository currently targets **.NET 11** for its application/core projects. The separate UnderstandMe library starts from a **.NET 10 skeleton**; cross-version packaging and integration will be handled explicitly during implementation, rather than assuming they already work together.

## Build and run the current foundation

Install a compatible .NET SDK for the projects in this repository (currently .NET 11, including preview tooling where required).

```bash
dotnet build Afutrans.slnx
dotnet test Afutrans.slnx

dotnet run --project WebTranslator
dotnet run --project Translator
dotnet run --project TUI.Translator
dotnet run --project TUI.Translator -- --culture cs
```

These commands target the **existing application skeletons**. They do not start a complete translation system. Historical test results are recorded in the implementation plan; **this README update has not run a build or test suite**.

## Development approach

Implementation will proceed in small, tested, documented increments. In particular, universal translation contracts, providers, placeholder processing, scheduler persistence, QueueMaster handoffs, work reporting, and editing flows must be agreed before their code is introduced. The Dita project's existing [LibreTranslate integration and localization tests](https://github.com/afrowaveltd/Dita) are a useful reference, not a ready-made multi-provider architecture to copy wholesale.

Keep this README focused on **what Afutrans is for**, and use the more detailed design documents to explain contracts and edge cases. Explicitly distinguish **implemented**, **in progress**, and **planned** behavior as development advances.

## Bundled localization reference data

The [`Afutrans.Core/Jsons/Cldr/`](Afutrans.Core/Jsons/Cldr/) directory contains **pinned Unicode CLDR 48.2.3 supplemental JSON reference data** for future offline locale/plural-rule resolution, country-language catalog enrichment, and grammar metadata. The original `Afutrans.Core/Jsons/languages.json` is preserved without modifications.

These are upstream reference datasets, **not implemented translation dictionaries, rules evaluators, or running services**. Future opt-in importers and adapters will keep simple flat `en.json`, `cs.json`, and other dictionaries usable without CLDR or grammar metadata. See [CLDR reference data and import plan](Docs/CLDR-Reference-Data/en.md).

## Documentation and project status

- [Afutrans implementation status](IMPLEMENTATION_STATUS.md) — verified baseline, changes, next continuation point.
- [Current foundation implementation plan](IMPLEMENTATION_PLAN.md) — historical UI skeleton work package and test notes.
- [UnderstandMe overview](https://github.com/afrowaveltd/Toolbox/blob/master/UnderstandMe/README.md) — planned reusable translation toolkit.
- [UnderstandMe architecture](https://github.com/afrowaveltd/Toolbox/blob/master/UnderstandMe/Docs/Architecture/en.md) — component responsibilities and boundaries.
- [Afutrans host and integration design](https://github.com/afrowaveltd/Toolbox/blob/master/UnderstandMe/Docs/Afutrans-Integration/en.md) — detailed planned operator workflows.
- [UnderstandMe implementation status](https://github.com/afrowaveltd/Toolbox/blob/master/UnderstandMe/IMPLEMENTATION_STATUS.md) — state of the reusable library.

Afutrans and UnderstandMe are developed in the open as part of the Afrowave software ecosystem. The product description will evolve alongside implementation; **features described as planned are not promises of currently available functionality**.

### Dedicated worker lanes and customer-owned providers (planned)

**Current hosted direction:** keep Afrowave's internal realtime capacity and the CommunityFree deferred worker pool **separate** from customer-paid work. Instead of selling better placement in the same community queue, assign a dedicated tenant queue lane bound to a separately funded VPS/LibreTranslate worker, or accept a compatible **customer-owned** LibreTranslate endpoint, cloud translation account or AI backend such as Ollama without charging them to rent our servers. Only the corresponding owner can use that backend; source text, credentials and result access remain tenant-scoped. Public reporting should account transparently for contributed amounts and actual VPS/operations/provider costs; no artificial per-word usage cap for customer-owned hardware, but physical resources and abuse protection are finite.

See [Dedicated Capacity, BYO Backends and Transparency](Docs/Dedicated-Capacity-BYO-Backends-and-Transparency/en.md). This supersedes the earlier proposed paid priority over shared CommunityFree work.

### Adaptive community waiting and shared backend capacity (planned)

Default public service direction: **no arbitrary total-translation credit wall**; use honest, finite durable deferred job admission and slower processing under load, with fair backpressure/abuse safeguards. Queue lanes remain protected, but an owner may voluntarily lend an **idle** bounded LibreTranslate/AI endpoint buffer to another authorized lane; reclaim future slots when needed, without claiming that already running work is instantly preemptible. A user may run their own realtime provider and choose community/other eligible engines for deferred work. Two-way AI ↔ LibreTranslate resource sharing is optional, privacy/consent/budget-limited in each direction. A self-hosted backend may connect via authenticated outbound agent even without inbound ports. See [Cooperative Backend Sharing](Docs/Cooperative-Backend-Sharing/en.md).

