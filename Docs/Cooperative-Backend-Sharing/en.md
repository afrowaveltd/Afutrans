# Cooperative Backend Capacity, Adaptive Waiting and Reciprocal Sharing

**Status: agreed conceptual direction, 2026-10-20; no runtime implementation or new tests.** This refines [Dedicated Capacity, BYO Backends and Transparency](../Dedicated-Capacity-BYO-Backends-and-Transparency/en.md), without weakening lane isolation, private default, or real-time semantics.

## Product policy: prefer waiting over usage-credit exhaustion

- The operator's preferred CommunityFree user experience is **no arbitrary cumulative monthly/total translation allowance that turns off service when consumed**. Rather than 'you used 10,000 requests, now pay', offer durable explicit **deferred** work and tell the user when processing is slow. Avoid artificial hard translation-volume limits for a customer's own engine/VPS too.
- This does **not** promise infinite storage/throughput/admission. Apply *adaptive backpressure*, weighted/fair per-caller concurrency, pending work/bytes limits, time-to-live/retention, provider hard spend ceilings and protection against abusive floods. If no durable capacity remains, return explicit `RetryLater`/`Busy` instead of accepting work and losing it. Prefer throttling intake and processing speed to arbitrary total count quotas. Protect previously admitted work.
- Submitted deferred work gets a durable receipt/job ID, transparent queue state and eventual completion **where actual resources and retention policy allow**. P0 realtime remains **volatile automatic-only**, never silently converted into a long-lived job, queued to human workers or stored as private chat content. If best-effort realtime cannot be delivered immediately, return accurately specified source/unavailable, or let the **caller separately opt into a new deferred request**.
- Community is a distinct protected capacity domain: payment does not buy queue-jumping or take its reserved workers. Afrowave's own services have internal first-party realtime resources. An independently funded tenant may instead operate a separate bound VPS/LibreTranslate or its own cloud/AI provider.

## Policy-controlled cooperative capacity lending

- QueueMaster's **separate queue lanes** (AfrowaveInternal, CommunityFree, TenantDedicated) keep their access policy and priority classifications. **Backend/worker capacity** may be **temporarily shared between lanes by explicit, revocable, resource-owner policy**. Lending one idle translator buffer is not merging queues or granting the recipient general rights to the owner's lane/credential/data.
- Example A: Afrowave internal P0-compatible workers have genuinely spare capacity. Operator can loan *specific available slots* to CommunityFree on a **non-guaranteed, best-effort** basis. This improves free throughput without selling priority or diminishing configured protected minimum first-party capacity.
- Example B: a tenant owns Ollama/AI and voluntarily contributes suitable idle translation/review capability to CommunityFree, while using explicitly offered Afrowave LibreTranslate capacity for its ordinary translations. **Reciprocity is optional**, may be asymmetric, and is never an automatic contract for unrestricted usage. Both owners grant and may withdraw sharing independently.
- Example C: a customer uses their own backend for realtime, while routing their explicit eligible non-realtime work to allowed community/Afrowave offered capacity. Check per-operation and per-lane entitlement, capabilities, provider costs, privacy and the distinction between *protected* capacity and *donated idle* capacity.
- Example D: an existing owner-operated LibreTranslate instance is reachable via a direct HTTPS URL or an authorized outbound-only agent/tunnel when inbound ports are unavailable; no obligation for every customer to expose a public inbound port. Authenticate owner, endpoint and each lease; bound egress and forbid turning an agent into a general SSRF/network gateway.

### Loan lifecycle and buffers

1. Resource owner opts in to offer selected worker slots/endpoint capability to a chosen lane/caller group (`CommunityFree`, one tenant, approved apps, etc.), with optional time window, concurrent-slot ceiling, processing-cost ceiling, supported formats/languages/privacy and a minimum **reserved owner capacity**. Default is **private/not lent**.
2. QueueMaster admits eligible work according to the **borrower's own lane**. When the donor's endpoint buffer has a slot genuinely free, the scheduler may grant a **short-lived processing lease**, scoped to this buffer + borrower lane + compatible operation + owner consent. No unrelated access to donor credentials or dictionary results.
3. The buffer pulls/accepts one authorized compatible item according to its agreed contract and per-buffer FIFO. Shared-lane work cannot consume a protected slot and cannot indefinitely hold up the donor's own realtime readiness. Prefer short bounded work units and avoid filling a long buffer with borrowed work.
4. If the donor's own high-priority demand returns, **stop admitting new borrowed items immediately** and reclaim the slot after accepted in-flight work finishes. Do not claim immediate zero-latency preemption of an already-running LibreTranslate/AI call. Preserve a permanently reserved minimum slot for true critical realtime when required, or use cancellation only where safe and provider billing/idempotence support it.
5. On revocation, expiry, backend disconnect, failure or cost-budget exhaustion: no new borrowed dispatches; finish/cancel outstanding calls according to explicit policy; preserve or requeue durable eligible CommunityFree work under its original lane, with idempotency and no secret/result leakage. A P0 volatile borrower cannot silently turn into a deferred persisted task.
6. Observer/SystemMonitor can publish **aggregate** donated/borrowed capacity, available slots and revocations without raw text, credentials, tenant identities or private job detail. Operator dashboards show direct versus borrowed work with accurate ownership/cost.

**Queue lane and capacity ownership are separate dimensions.** A hosted customer may own a lane and two backends, one wholly private and one explicitly donated. Lending a compatible buffer is not granting the borrower permission to reconfigure that backend or read the donor's other jobs/results.

## Provider and privacy-specific rules

- A LibreTranslate provider, Google/Microsoft subscription, human worker or AI engine has **its own capabilities and authorization**. Owner may donate only allowed engines/features; a generic AI chatbot is not automatically a compliant translation worker. Paid-provider accounts require explicit donor-approved budget/allowance and account hard caps; merely owning paid Google does not grant the community unrestricted use.
- Before cross-owner processing, the request source's policy must **permit transmission to this processor** (including remote machine/AI processors, target privacy, document type and sensitive content). Provider-owner sharing consent and *content-owner processor permission* are **both required**. For private chats, never reinterpret donated capacity as consent for third-party AI/human exposure.
- Do not share tenant-specific provider API keys with borrowers; owner-hosted worker/agent mediates call, or use an appropriately isolated host-managed secret. No forced reciprocal provider sharing, and no cross-tenant result/dictionary/queue visibility.
- A voluntarily shared engine's owner may adjust availability, languages, capacity, spending or remove it. Results should preserve provider provenance for validation, corrections and accounting; public phrase sharing remains separate owner opt-in.

## Funding and realistic performance claims

- User observed that an entry VPS may be available for **roughly USD 6/month**; this is an *indicative author-supplied starting point*, **not a verified price or guaranteed sufficient LibreTranslate capacity**. Benchmark actual CPU, RAM, relevant models/languages, sustained throughput, network/backup costs and provider terms before setting an offer.
- A transparently documented rental can publish actual monthly charges and what they finance; **customer-owned provider usage does not require payment for a VPS**. Support/donations may fund extra CommunityFree capacity without selling preferential queue position. Include tax/fees and infrastructure overhead truthfully.
- If extra P0-capable internal capacity is offered publicly, it is explicitly **opportunistic, not an SLA**. Keep internal urgent workload reserved; public clients cannot rely on borrowed resources for mandatory realtime. Never advertise queued/delayed responses as realtime.

## Future tests

1. Submit many free jobs: prefer delayed durable admission and lower throughput over translation-credit exhaustion; when persistence/queue is saturated, reject honestly with retry guidance without losing already accepted jobs.
2. Donate one idle internal backend slot to CommunityFree; when internal P0 arrives, no **new** community jobs occupy that slot, bounded in-flight borrowed item drains, and the protected internal slot stays available.
3. User shares an AI endpoint to CommunityFree only for consented language/format privacy categories. Test prohibited private chat, unsupported HTML and unpaid cloud budget exhausted; no unauthorized calls.
4. Tenant owns realtime engine and has explicitly authorized alternative non-P0 community processing; tasks use independently eligible buffers and there is no silent access to protected private capacity.
5. Voluntary mutual offer Google/AI ↔ LibreTranslate: verify permissions/cost owner per direction; one party's revocation never grants access to the other's unrelated tenants or credentials.
6. Home LibreTranslate with no inbound ports uses authenticated outbound agent; job identity, source privacy, results, reconnect/replay and quota/egress isolation hold.
7. Community/tenant backends fail or loans expire while durable jobs queued/leased: jobs recover idempotently in originating lane, with accurate source/result access and provider charge bookkeeping.

**No scheduler, buffer leasing, outbound agent, hosted pricing, or user interface has been implemented by this document.**

## Contributions never buy free queue position or personal profit (2026-10-20)

**Highest-priority hosted policy:** CommunityFirst, free when feasible and no hard total credits imposed as a product lever. A shortage causes honest slower completion/backpressure, not a forced paid upgrade. Idle worker donations are voluntary and privilege no person above a community peer. Financial donations are likewise voluntary and support documented Afrowave running costs such as adding VPS, prepaid AI tokens or securing shared infrastructure; no one personally profits from the pooled contributions.

Actual gross receipts, purpose restrictions, payment/FX/tax fees, credited AI tokens, VPS invoice/active service and unspent earmarked balances must reconcile and be visible in a privacy-safe community overview and contributor receipt. See [Community-First No-Profit and Transparent Funding](../Community-First-No-Profit-and-Transparent-Funding/en.md). **Design only.**

