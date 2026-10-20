# Dedicated Queue Lanes, Bring-Your-Own Backends and Transparent Cost Recovery

**Status: agreed product/architecture direction, 2026-10-20; not implemented.** This note **supersedes** the earlier notion of selling higher priority over community work inside the same shared worker pool. Design decisions on exact API, payment processor, hardware price and guarantees remain open.

## Non-negotiable product philosophy

- **No paid queue jumping in the Community pool.** Community users share a designated, free, finite worker pool with its own admission limits and deferred non-P0 scheduling. Buying a plan must not reduce the community's reserved worker capacity or its fairness.
- **Afrowave Internal is separately protected.** Our Id, Afutrans and other first-party services can use a reserved internal lane/workers including genuine P0 RealTime. A public free request does not acquire this realtime resource merely because a client sets a priority flag.
- **Commercial means a separate tenant lane and separately funded capacity**, not a privileged position in the community backlog. Optionally deploy a customer-dedicated VPS with its own LibreTranslate workers, or register customers' existing, independently funded backends. Commercial workloads must not consume protected Afrowave/community worker slots without an explicit opt-in donation/overflow policy that preserves their reserved minima.
- **Bring Your Own (BYO)** does **not** intrinsically require a fee: an authenticated customer with an existing compatible LibreTranslate endpoint, Microsoft Translator/Google Cloud account, hosted AI account, Ollama instance or another adapter may connect it without buying our VPS. They consume their own backend quota/compute, while Afutrans provides authorized orchestration and only bounded management/queue/validation service. Keep our control plane from being overloaded by supposedly free BYO traffic.
- **Transparency over profit maximization.** Show funds accepted for hosted VPS/capacity, actual VPS/network/storage/operations/provider charges and capacity funded. Do not promise '100% paid to VPS' if unavoidable payment-processing fees, taxes, backups, networking or necessary administration costs apply: publish honest line items and disclose whether we are subsidizing anything or charging setup/support.

## Proposed lane and backend layout

```text
Authenticated or explicitly anonymous user/app
      |
      v
Service intermediary / Id (optional) / ClientPolicy / target access checks
      |
      v
Afutrans Orchestrator -> QueueMaster (dynamic queue lanes)
      |
      +-- AfrowaveInternal lane (P0 RealTime + allowed P1-P4) --> reserved Afrowave workers
      +-- CommunityFree lane   (P1-P4, deferred only)        --> protected community workers
      +-- Tenant:<id> lane    (configured P0-P4 subset)     --> tenant-owned or rented workers/accounts
      +-- ... additional explicit tenant lanes as registered/authorized
```

Each lane is a **distinct scheduling and isolation domain**, with queues/priority policy appropriate to its own workload; existing P0–P4 describe *job urgency inside a lane*, not a payment rank. QueueMaster can be one reusable scheduler component serving multiple lanes or isolated instances with the same contract; do not assume every tenant must have a dedicated scheduler process. A queue alone cannot provide resource isolation: **worker pool/endpoint binding, concurrency, quotas, task ownership, memory and network budgets** must also be isolated.

### Suggested lane configuration dimensions (not fixed C# APIs)

| Concept | Purpose |
| --- | --- |
| `LaneId`, `Owner`, `IsolationMode` | Internal, community or tenant-specific queue/worker domain; owner scoped to Id/host grants |
| `AllowedPriorities` | Valid P0-P4 subset, not a new universal `Paid` enum |
| `AllowedEndpoints` / `WorkerPoolId` | Explicit private worker/account bindings; prevent accidental public fallback |
| `CapacityBudget` | Concurrency, queue depth, memory/throughput and finite backpressure even for dedicated machines |
| `AuthorizationPolicy` | Translation permission distinct from connector/target admin permissions |
| `FallbackPolicy` | Default **none across isolated pools**; opt-in safely only if source and destination owners allow resource sharing |
| `PersistencePolicy` | P0 volatile/automatic-only and P1-P4 permitted durable mode; no silent P0 to Human or persistent backlog |
| `EndpointHealth` / `CostOwner` | Health, billing, hard provider budgets and resource owner |

Preserve priority snapshots, retries, idempotent work claims, small translator FIFO buffers and durable non-P0 work semantics from original QueueMaster. Dynamic **creation/removal of lanes** requires authorization, consistent admission, graceful drain/cancel with explicit policy, no orphaned persistent work and restart-safe restoration. Quorum/leader and multi-instance claims cannot let two workers complete the same job twice.

## Three real modes, not three service classes

1. `AfrowaveInternal`: guaranteed **reserved** service capacity and allowed realtime for the platform's own applications. If operators intentionally sponsor the public with surplus, it is an explicitly accounted donation, not a hidden preemption.
2. `CommunityFree`: transparent bounded admission of public/authorized **deferred** translation; queue work fairly inside this lane, optionally share a separately budgeted community LibreTranslate pool. Existing previously admitted community work must not be starved by a new commercial tenant.
3. `TenantDedicated`: separately bound queue lane using (a) **managed customer's VPS/LibreTranslate instance**, (b) **customer-supplied LibreTranslate instance**, or (c) **customer-supplied metered provider/AI account**. More than one backend can be granted to the same tenant according to capabilities, costs and privacy. Tenant may choose realtime if an automatic compatible worker is genuinely available.

The user may configure an endpoint mapping without buying a hosted machine. If they already pay Google or Microsoft directly, that provider remains **the customer's billing responsibility**. A self-hosted on-premises Ollama/LibreTranslate machine remains their resource. Id grants and Gateway route admission do not transfer ownership of their credentials/dictionaries to the platform.

## Bring Your Own Provider (BYOP) onboarding

- Owner authenticates to the service intermediary/Id or standalone host, proves authorization for their `Translation Target` or account, and registers a provider endpoint under a specific tenant/lane. The provider family/adapter and supported capabilities (text/HTML/document/chat, locale pairs, rate/batch, AI vs conventional) are declared and tested before eligibility.
- Endpoint may be an explicitly configured HTTPS backend, a self-hosted LibreTranslate API, an operator-approved HTTP/FTP legacy/test endpoint under isolated trust policy, an outbound client-controlled agent/tunnel (useful for private infrastructure), or a cloud provider account referenced through scoped secrets. **Never ask customers to send raw provider keys or target database passwords to Id**; keep credentials at the owning integration/gateway vault or in an owner-controlled agent.
- Apply egress restrictions, SSRF protection, tenant-level rate/concurrency, request/body limits, bounded network retries, credentials redaction, TLS verification where configured, and proper isolation between tenants. If customer chooses an insecure test connector, disclose risks without allowing it to compromise our platform.
- `ClientPolicy` binds provider endpoint/account IDs to the owning tenant and acceptable privacy class; use a credential vault/opaque secret reference and never forward arbitrary `ProviderId` or attacker-chosen host URL into an unrestricted network fetcher.
- Provider costs/meters belong to **the account owner** (especially cloud per-character/token plans), with optional usage reports for the customer and per-account hard caps; do not bill someone else or silently fall back to Afrowave's own paid backend if their quota expires. Normal failure outcome is queue/wait/explicit unavailable, with **opt-in** cross-account fallback only.
- For AI integrations use the same **registration/isolation/capacity plumbing** but distinct provider capabilities and policy/adapter. AI chat, localization suggestions, document translation, semantic review and privacy consent differ; not every AI model is interchangeable with a translator.

## Paywall as a transparent capacity ledger, not a pressure tactic

Offer a **publicly understandable operating-cost dashboard** (the label `paywall` is a product presentation choice):

- Example offer: 'Fund one VPS of the following published specification at X/month; we set it up, bind it to your private lane, maintain it as described, and route your eligible translation requests to it.' **X is determined by actual quotes and approved included costs**, not guessed in design docs.
- Display the amount received for each selected funding period, separated by donor-supported Community pool versus per-tenant dedicated rental. Include VPS cost, network/backup costs, payment fees/taxes when relevant and any explicitly included administrative time/support. Clarify whether balances/rollovers are reserved for future infrastructure.
- For tenant VPS plans avoid artificial *word count/translation count* limits where backend capacity and operating costs allow, but **do not promise literally unlimited throughput, latency or provider usage**: bound work by finite VPS CPU/RAM/storage/network, reasonable abuse prevention, hard cloud-provider quotas and honest queue backlog.
- Dedicated paid capacity is unavailable when its VPS/provider is down, unless a genuinely provisioned and owner-authorized fallback exists. A separate operational dashboard shows uptime, queue depth, processed requests, latency, cost and supported provider without disclosing submitted text or customer identities.
- A client using its own healthy compatible backend does **not** owe the project a VPS rental or mandatory commercial subscription. Any future optional management/support cost would be explicit and not smuggled in as per-translation fees.

## Runtime and management isolation

- Keep the intermediary/Gateway a controlled public interface and Afutrans in a private network for hosted deployments; both Gateway and Afutrans application handlers check target-bound rights. Community translation/public API permissions never grant ability to add a backend or inspect arbitrary endpoints.
- Protect Afrowave's control plane from a dedicated customer's infinite submissions using admission cost, finite queues, memory limits, API worker caps, outbox/polling backpressure and authentication. **No arbitrary usage quota** does not mean unbounded control-plane compute.
- Dedicated tenant job/result isolation is mandatory: one tenant must not see another's text, queued work, provider keys, dictionary, artifacts, queues or operational traces. Optional public phrase publication remains a separate owner opt-in.
- Translators remain optional plug-ins; Human-only and offline/dictionaryless UnderstandMe must not require any paid capacity, Id, Gateway, cloud or multi-tenant lane host.

## Future tests

1. Ten community users and a heavy paid tenant submit continuously: paid jobs consume **zero** community-reserved worker slots, and community throughput is no worse due to paid dispatch (apart from explicitly tracked shared-control overhead).
2. Only internal first-party clients can access reserved internal P0 endpoint capacity; spoofed priority, `LaneId` or `EndpointId` from anonymous clients is rejected.
3. Tenant with own LibreTranslate endpoint can create an approved lane without subscription/payment; failed tenant endpoint does not trigger silent use of Community/Afrowave workers.
4. Tenant with owned Google/Microsoft plan is billed by the correct provider account, uses only its explicitly permitted endpoint, and respects its own account hard caps and data privacy.
5. Tenant with Ollama/AI backend can use explicitly supported AI translation/review capabilities, without pretending every backend is a compatible simple text translator.
6. Create, restart, pause, drain, resume and remove an authorized dynamic lane while in-flight/durable jobs exist; no lost, duplicated or leaked jobs, no cross-tenant ACL bypass.
7. Public cost dashboard agrees with real contribution receipts/hosting charges and never exposes donor personally identifying information or tenant raw text.
8. Dedicated VPS failure causes tenant-specific degradation and honest capacity reporting; community/internal pools continue with their reserved capacity.

**Implementation status:** product and architectural documentation only. No dynamic lanes, BYO provider portal, tenant deployment, payment processor, transparency dashboard or new tests are implemented in this change.

## 2026-10-20 — voluntary spare-buffer exchange and prefer-delay policy

**Owner clarification:** prefer **slower processing and visible waiting** over arbitrary cumulative translation quotas that force free users to pay or stop after credits run out. Explicit deferred CommunityFree tasks use safe durable admission and fair processing, subject to real queue/storage/abuse constraints and honest backpressure rather than unlimited acceptance.

QueueMaster's isolated lanes **may borrow spare backend buffers** only by a separate owner-approved, revocable, non-guaranteed capacity policy. When Afrowave Internal has extra P0-capable worker slots, idle capacity may be **offered to community** without changing reserved internal realtime minima. Reclaim at the next safe slot boundary, not by falsely claiming an in-flight non-preemptible provider call can be instantly interrupted. Owner-supplied AI ↔ LibreTranslate sharing is **voluntary in each direction**, subject to both the donor's spend/privacy allowances and the content owner's processor consent. A client can route its own realtime to its own engine and approved other work to borrowed/community endpoints.

Own server need not expose inbound ports when an approved **outbound connector agent** is available. The owner's rough estimate of **USD 6/month** for a basic VPS is illustrative only; machine suitability/pricing and actual cost transparency require measurements. Full proposal: [Cooperative Backend Sharing](../Cooperative-Backend-Sharing/en.md). **Design only.**

