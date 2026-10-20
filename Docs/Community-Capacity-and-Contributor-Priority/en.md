# Community Translation Capacity and Contributor-Funded Priority

**Status: product design agreed in principle, 2026-10-20; not implemented.** This document describes a preferred *hosted Afutrans operator policy*, not a mandatory rule for all UnderstandMe, Afutrans or self-hosted deployments. Final limits, payment model, amounts, promises and availability objectives remain to be defined.

> **Decision update (2026-10-20):** The earlier `ContributorPriority` proposal to give paid clients higher priority on **shared community workers** is superseded. The owner explicitly chose isolated **CommunityFree**, **AfrowaveInternal** and **per-tenant dedicated/BYO queue lanes and worker pools**, so paying does not cause queue-jumping or reduce the free community's reserved capacity. No fee is required to connect an already-owned compatible backend. Canonical updated design: [Dedicated Capacity, BYO Backends and Transparency](../Dedicated-Capacity-BYO-Backends-and-Transparency/en.md). The older profile/dispatcher examples below are historical exploration; where they conflict, the new lane-isolation design governs.

## Intent

Afutrans can provide useful shared translations without charging for every phrase. Public infrastructure nevertheless costs money: VPS, network, storage, CPU/RAM, reliability, optional dedicated LibreTranslate instances and operating reserves. The author favors **free deferred community work** and a voluntary/customer **contribution-funded faster service**, with actual contributions used to sustainably add dedicated worker capacity rather than maximizing revenue.

Prefer describing the optional service as **community free / contributor priority / dedicated capacity**, not artificially restricting the translation engine itself. An owner may offer a different model, sponsor all tiers, run a free local-only installation or not publish a public API at all.

## Distinct dimensions: visibility, authorization, execution urgency and capacity plan

- `APIExposure`: enabled internal-only, authorized published, or deliberately public/anonymous route. This is unrelated to payment entitlement.
- `OperationPermission`: which user/application may translate, create targets, read/edit private dictionaries, change storage credentials, use provider accounts or administer the service. Anonymous access to permitted ordinary text translation never implies permission for storage/tenant management.
- `ExecutionSemantics`: P0 `RealTime` (volatile automatic-only immediate path) versus explicit **durable deferred** translation jobs admitted on non-P0 priority P1–P4. A slower result is not labelled `RealTime` simply because it arrives later. No P0 request silently becomes a persistent Human/background job; the caller must explicitly submit an eligible deferred request.
- `ServicePlan` / `CapacityEntitlement` (provisional terms): operator-defined allocation of reserved worker slots, request burst/rate, maximum queue length, queue wait target, and provider eligibility. A contribution can buy capacity/priority under a service agreement; it does **not** redefine P0–P4 or turn a Human translator into instantaneous compute.
- `ProviderAccountPlan`: cost paid by Afutrans to Google/Microsoft/other paid engines, or fixed infrastructure costs for self-hosted LibreTranslate. These are **not** equal to subscription revenue or a caller's paid plan.

## Candidate hosted profiles (examples, not hard-coded limits)

| Profile | Intended access | Work and resource treatment |
| --- | --- | --- |
| `CommunityFree` | Anonymous or registered requests on approved public methods | Explicit durable deferred translation (e.g. P3/P4 according to request and host policy); bounded queue, source-size/rate/quota limits and spare or minimum reserved service capacity; no instant-result guarantee |
| `ContributorPriority` | Registered and verified plan entitlement, optionally through first-party Id/gateway | Faster admitted **compatible** jobs, a bounded priority ceiling and/or allocated worker capacity; optional realtime only where an actual compatible, available P0 automatic endpoint and capacity policy supports it |
| `DedicatedCapacity` | Specific subscribed application/organization | Reserved or independently deployed LibreTranslate worker pool with metered capacity and a defined share or target service level; account/tenant permissions and global hard caps still apply |
| `SponsoredCommunity` | Operator or donor contributes a funded shared pool | Reserve capacity or expand free allotment transparently; does not imply donors can inspect client requests |

A contributor's benefit may be **higher throughput / more available workers / a shorter typical wait**, not a guaranteed translation quality improvement. Never claim a fixed response time or SLA unless deployment monitoring, sufficient worker pool and enforceable contract support it.

## Implementation without rebuilding QueueMaster

1. The optional API Gateway resolves a trusted caller, an optional Id grant and a verified service/capacity entitlement, or assigns the explicitly configured `PublicAnonymous` policy. Do not authorize paying priority from a caller-provided `planId`/`ClientId` alone.
2. The Afutrans host Orchestrator validates the requested operation, tenant/target permissions, privacy requirements, provider eligibility, client quota and global/provider-account hard budgets. It captures the normalized effective request and reasoned priority ceiling. **Do not add new per-customer central queues by default.**
3. A `CommunityFree` caller asks for a deferred result and receives a durable `Accepted` / job receipt, status polling or optional authorized business SignalR progress, then later retrieves the verified translation. This is explicitly non-P0 and does not promise instant completion.
4. A verified priority caller can request faster processing on an admitted P1/P2 job **subject to its specific policy**, or a separate P0 real-time request only where compatible automatic worker capacity and realtime policy genuinely allow it. QueueMaster still owns the original five priorities; Translator workers pull compatible jobs and their bounded buffers remain FIFO.
5. Rate/concurrency/queue-item caps and reserved worker slots or weighted capacity allocation prevent a large public stream from exhausting the host. If demand exceeds configured capacity, respond clearly with backpressure, approximate wait/status, or reject new work instead of accumulating an infinite backlog.
6. **Anti-starvation policy:** give admitted community work an administratively configurable minimum share, bounded waiting opportunities, aging/fair scheduling or a separate capacity reservation as deployment design warrants. The five priority semantics and no-reorder worker buffers must still be respected; any implementation of aging or fair share belongs at the orchestrator/dispatch/eligible-worker layer and needs explicit tests, not an accidental QueueMaster rewrite.
7. Support independently registered LibreTranslate endpoints and worker pools; newly funded VPS increases real usable resource/capacity entitlements, with failover/health/cost measurement and no assumption that adding a host instantaneously produces zero-latency translation.

## Financial sustainability without coupling payments to the engine

- Distinguish **customer/sponsor contributions and plan entitlement records** from provider-account usage and actual infrastructure costs. A `UsageLedger` that accounts for Google characters or LibreTranslate CPU must not be repurposed into an unverified payment ledger.
- Identity/Id may grant service entitlement or deliver verified membership facts; payment collection/invoice processing is an optional separate application or backend, **not** a mandatory UnderstandMe dependency. No external payment gateway or vendor is selected.
- Keep transparent metrics: worker availability, queue delays, utilization by free/priority plan, operating cost, paid-provider spend, contributor-funded capacity added and reserved community share. Public reports are aggregate and omit private translated text and account/payer identifiers.
- If a donor explicitly funds a community pool without demanding priority, allow those resources to benefit free requests. Do not implicitly spend on paid cloud translation accounts or additional VPS without operator-authorized budgets.
- Administrative ability to pause paid queues, set maximum processing cost, reduce provider access or throttle abuse applies equally to entitled paying callers. No entitlement overrides data privacy, Id authorization, account hard limits or internal safety.

## Availability and user expectations

- Free deferred work is **free but capacity-bounded**, not a promise of infinite jobs or infinite waiting. Report queue admission and completion states honestly; reject with usable diagnostics when no capacity is available rather than accepting unfinishable work.
- Explain which service features are sponsored by contributions and why priority needs real compute capacity. If a tier is truly a paid contract, disclose specific terms, expiry, service levels and any refund/cancellation mechanism; the initial product concept intentionally does not freeze them.
- Anonymous free callers may be harder to meter across IPs; use transparent limits and abuse protection without behavioral tracking or third-party cookies. Paid/priority entitlements require trusted identity and authorization through optional Id and the service intermediary.
- A hosted operator can switch off anonymous requests, paid upgrades or all remote access without breaking offline dictionaryless localization, self-hosted free translation or manual Human workflows.

## Acceptance tests to design later

1. Anonymous caller is authorized for one deferred translation, gets a durable job ID and eventual result without passing through P0, privileged target storage, Human inbox or unrelated private dictionaries.
2. Anonymous request attempting SQL target registration or arbitrary priority self-elevation is rejected at gateway and host command authorization.
3. Verified contributor gets its configured bound on priority or dedicated capacity without exceeding global provider/billing budgets; `Priority=P0` on a request alone is not proof of paid entitlement.
4. Run many concurrent public free requests and a defined priority load; global queue limits hold, internal work is preserved, and community work demonstrably makes progress under the selected fairness policy.
5. Remove all automatic backends: existing Human-only non-P0 work can continue under permission/consent; P0 remains inactive and never silently queues private chat content to humans.
6. Dedicated LibreTranslate VPS dies: available capacity and admission/routing adjust; no duplicate accepted jobs or hidden billing and no infinite request retention.
7. Contributor plan expires while job is pending: policy precedence for already accepted work and future dispatch is explicit and reported; it never bypasses revoked source/target permission.
8. Track separately estimated infrastructure costs, actual provider-account charges and contribution payments, without logging text or payment secrets into public monitoring.

**Status:** documentation only; no subscription, payment processor, paid entitlement, worker allocation, multi-VPS capacity reservation or API gateway was implemented by this design.

## Current policy refinement — no artificial translation credit exhaustion (2026-10-20)

**Superseding operator preference:** avoid hard per-month/per-lifetime **translation count/word allowances** as the normal free-community experience. Prefer explicit durable deferred requests that become slower when resources are busy, with bounded fair intake/concurrency/storage and clear queue status. Admission must fail honestly on actual capacity/safety limits rather than inventing unlimited persistent storage. Genuine P0 cannot silently turn into deferred persistent or Human work.

Optional best-effort idle-worker *donations* do not grant paid priority in the common pool. Separate protected lanes remain, and dedicated tenants may choose to lend authorized endpoint buffers temporarily or supply their own backend. See [Cooperative Backend Sharing](../Cooperative-Backend-Sharing/en.md). All older shared-queue paid-priority and arbitrary free per-period quota examples are nonbinding and superseded where conflicting.

## Governing no-profit funding policy overrides paid-tier examples (2026-10-20)

**Do not implement historic sample paid priority or arbitrary free daily/monthly credit limits as product defaults.** Owner explicitly rejects hard pay-to-continue mechanisms as both initial and final community model. Provide everything feasible free under equal community rules, defer and slow requests when limited, and allow *voluntary* donations solely for documented actual infrastructure, paid AI tokens and other necessary service costs, with complete public accounting to the last currency minor unit.

Detailed one-time vs recurring contribution purposes, earmarking, allocation vs actual spending, fees/taxes, unspent balances and privacy: [Community-First No-Profit and Transparent Funding](../Community-First-No-Profit-and-Transparent-Funding/en.md). No personal profit, no surprise charges, and no advantage over CommunityFree workers for donors. Earlier plan examples remain history, not a contradictory implementation requirement.

