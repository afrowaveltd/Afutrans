# Community-First Access, No Personal Profit, and Transparent Funding

**Status: confirmed product and ethical requirements, 2026-10-20; not implemented.** This is the **governing community-funding policy** for the proposed Afrowave-hosted Afutrans service. It takes precedence over earlier illustrative `PaidClient`, `ContributorPriority`, hard monthly free credits, quotas as pricing, paywall and commercially ranked shared queues wherever those examples conflict. A standalone, private or self-hosted installation may have its own administrator-defined deployment policy; it must not be forced to install payments/Id.

## Purpose: offer useful service, not monetize access

- Afrowave's goal is to provide **everything feasible for free under fair, equal community rules**, with no expectation that a user must pay or donate to receive a basic translation/localization service. This free-first intention applies at launch and as a long-term direction, not merely an introductory or trial tier.
- **No personal enrichment from community contributions:** all voluntary support is assigned to and accounted against real Afrowave community service operations, hosting, provider tokens, hardware, networking, storage, backups and other necessary disclosed service expenses. No secret profit margin, artificial scarcity, unsolicited upsell or payment-for-position in a shared community queue.
- A contribution is **voluntary** and can be purpose-restricted by its sender (e.g. community LibreTranslate VPS, extra paid AI token credit, backups/redundancy, another named service), or given to an explicitly described general operations pool. Respect purpose restrictions; if a purpose becomes infeasible, seek contributor permission to redirect funds or handle refunds/held balance according to published terms.
- **No fixed credit-wall default or end-state:** free users should not be told their monthly/total translation allotment is used up merely to induce payment. Prefer durable deferred non-P0 admission, honest backlog/wait, lower processing rate and fair dispatch. When *physical* storage/capacity or abuse makes accepting another request unsafe, respond truthfully with backpressure/retry (no false durable ACK). Distinguish unavoidable technical protection from deliberately imposed monetization ceilings.
- **Equal treatment inside CommunityFree:** no monetary donation, contribution or subscription buys preferential placement ahead of a less affluent user in that common worker pool. AfrowaveInternal retains its separately reserved service-critical P0 capacity; dedicated/rented or owner-supplied resources have separately authorized lanes, never a right to displace free community work.
- When idle workers/AI are voluntarily lent to the community, benefits are shared under the owner-approved privacy/cost policy and are best-effort. No contributor can demand access to another user's private text, provider credentials or dictionary as a consequence of funding.

## Contributions are converted into actual operating value

Examples of eligible applications:

| Contribution purpose | Verifiable resulting value |
| --- | --- |
| One-off contribution to paid AI capacity | Actually purchased/credited provider tokens, characters, requests or balance, including real provider tariff and fees |
| Recurring contribution to community translation | An additional rented or renewed LibreTranslate VPS/worker and documented running period, network and operational charges |
| Dedicated tenant hosting at real cost | Named VPS configuration and its cost for the agreed term, with resources bound to the tenant's lane; **not** a premium entitlement to community capacity |
| Community operations | Disclosed infrastructure, backups, domain/security/monitoring expenses needed to keep donated services running |
| Bring Your Own (LibreTranslate/AI/MS/Google) | Customer uses their existing backend; **no compulsory VPS rental or new donation** merely to register the integration |

Only apply funds to their approved purpose and actual necessary service operation. **A purchase is not the same as a promised allocation:** distinguish accepted donation, earmarked/held balance, pending purchase, actually paid expense, credits provisioned, activated service capacity and remaining funds. If the contribution is smaller than a VPS renewal, it stays visible as a purpose-bound balance until combined with others or otherwise lawfully used.

### Last-cent accounting: a publicly reconcilable ledger

Use exact currency minor units/decimal arithmetic (no binary floating-point money), stable idempotent transaction IDs, dates/currencies, evidence links and audited corrections. Proposed durable records:

- `ContributionReceived`: gross amount, currency, receipt period, an owner-visible private receipt ID and **public pseudonymous/anonymous transaction reference**; optional purpose or earmark and consent to publish recognition separately.
- `FeeCharged`, `TaxOrExchangeAdjustment`, `RefundOrChargeback`: actual external fees and taxes/exchange/returns, with sign and the party bearing them; do not conceal a fee by claiming 100% of gross was converted to compute if that is false.
- `FundAllocated` / `FundReserved`: move available balance to a named service cost center and approved purpose; **this is not yet a paid invoice**.
- `ExpensePaid`: provider invoice/hosting receipt, actual amount paid in its currency and reporting currency/FX method where applicable, cost center, covered period, nonsecret proof of purchase and specific benefit funded.
- `ServiceProvisioned` / `ProviderCreditVerified`: confirm resulting VPS period, worker capacity or posted AI provider credit. Separate this verification from financial expense, because a payment can fail to provision service.
- `BalanceRemaining`: still unspent earmarked and general funds, with clearly identified pending commitments and reconciled actual provider balance. Never silently treat unspent money as an operating cost or personal profit.

For each accounting period, publish an equation such as **Opening balance + gross contributions − actual transaction fees/taxes − paid service expenses − refunds ± documented adjustments = Closing balance**, reconcile against actual available balances and identify outstanding purchases/commitments separately. Clearly distinguish cash spending from provider credits consumed and prepaid token balances still available.

### What people should see

- **Community-wide transparent dashboard:** total money received, total actual paid running costs, remaining/unspent balance, purpose-restricted reserves, current VPS/server count and service status, provider prepaid credit/token balance where available, funded work/capacity and dated evidence (redacted invoices/receipts or verifiable aggregates). Show exactly what Afrowave costs, not a fabricated fixed tariff.
- **Contributor receipt:** private view with amount paid, real fees, earmark, allocation, dated paid expense or credited balance, resulting service value and current unspent remainder. If funds are pooled, show proportional/ledger-based allocation without making an unverifiable claim that their exact banknote purchased one machine.
- **Operator view:** reconciled complete entries, bank/payment/provider statements, supporting invoices, attribution and error corrections. Never erase/edit settled history invisibly; corrections are append-only reversals/adjustments with audit and explanation.
- **Privacy:** the public report does not reveal donor names, payment instruments, addresses, invoice secrets, raw translated text, accounts/API keys or tenant billing. Contributors may publish their name voluntarily but transparency of funds must not require publicity of people.

**Commitment and honesty:** no personal withdrawals, undisclosed commissions or markups. If an unavoidable card-processing fee, tax, currency conversion charge or genuine service expense exists, report it to the smallest supported currency unit, not as mysterious missing money. If legal/accounting requirements imply an expense that does not fit donor restrictions, disclose it and do not silently redirect earmarked funds.

## Capacity policy: communicate instead of restricting

- Explain **why** requests may queue or be slowed (currently available workers, shared ownership, failure, expense of AI tokens), what happens next, whether the request was accepted, and what optional user choices exist (wait, use own backend, donate idle compute, voluntarily contribute funds). Avoid manipulative countdowns or 'upgrade to finish your request' messages.
- Expose approximate queue status/wait only if based on measurements, never imply contractual realtime SLA from opportunistically borrowed first-party slots. Realtime for internal Afrowave and independent tenant-owned workers follows actual reserved capacity.
- Real hard provider spend budgets and catastrophic overload controls can be necessary to avoid ruining the project; they are **operational safety and sustainability measures** and not synthetic monthly free usage credits. Publicly describe the difference in nontechnical language. Do not persist private P0 by accident while slowing requests.
- Prefer throughput throttling, fair scheduling and bounded durable storage, then transparent `Busy`/`RetryLater` when protection is essential. The target is a service that tries to help **as many people as feasible**, not one that treats limits as desirable product features.

## Future acceptance tests and audits

1. Community user can submit successive eligible deferred requests without arbitrary period credit exhaustion; under load wait grows and real resource exhaustion uses honest backpressure, no lost ACK or forced payment prompt.
2. Simulate anonymous free, volunteer donor and managed tenant concurrently; no donation grants a higher queue position in CommunityFree; funding adds an independently attributable worker/backend.
3. Post donation in one currency, processing fee, provider token purchase in another currency, then an unspent remainder: public totals reconcile to minor unit and personal private receipt matches, with FX and evidence.
4. Dedicated-purpose funding cannot silently be spent on unrelated capacity. Show `Reserved`, `Paid`, `Provisioned`, `Unspent` separately; refund/redirect requires the specified authorization.
5. Provider tokens purchased but not yet used remain visible as prepaid service value; an actually failed VPS provision does not count as working capacity.
6. Public dashboard reconciles donations/costs without publishing personal donor information or backend API secrets. Correct an accounting error using immutable reversal/adjustment.
7. Entire system still works without payment implementation: optional funding and public reporting do not become prerequisites for local/offline UnderstandMe, standalone Afutrans, Id login or free core translation.

## Status and implementation boundary

**Only design requirements and ethics/product policy are recorded here.** No live donation/payment integration, ledger, invoices, public dashboard, exact cost pricing, VPS purchase automation or financial audit has been implemented or verified. Public transparency claims must wait for actual reconciled evidence.

## Contributor-defined split allocations and month-to-month carry-forward (2026-10-20)

**Confirmed additional requirement:** contributors may split **one payment across multiple explicit purposes**, for example a contribution of USD 10 allocated as **USD 5 to paid AI provider capacity and USD 5 to community server operation**. Record one immutable payment/receipt reference and multiple separately attributed `ContributionAllocation` entries whose total plus any **pre-disclosed and clearly accounted payment-fee treatment** reconciles exactly with actual funds. A purpose-restricted line must never silently be transferred to another purpose or privately withdrawn.

Fees must be explained **before contribution**: e.g. the disclosed fee may be borne by a separate general operations fund, added separately with consent, or distributed against the chosen gross allocations under a transparently approved rule. Do not promise a net USD 5 + USD 5 if actual received funds are lower and no other budget covers the difference. For multiple currencies, show received value, exchange evidence and exact minor-unit allocation/rounding.

**Never force immediate spending.** Contributions to server operation can be accumulated as restricted reserves for **future monthly bills** rather than spent merely because funds arrived. If five people collectively fund more than the current month's servers need, keep the excess visibly committed to the following months; the total available balance, already paid service periods, future reserved periods and still-unassigned/unspent amount remain separately visible. Do not label a future month's reserve a current month's cash expense or claim that a planned VPS is already deployed.

A contributor-facing future page should show their original gross payment, each purpose and allocated amount, applicable fees/taxes, actual invoices/AI top-ups, subsequent monthly balance movements, and remaining value. Community reporting shows aggregated transparent balances and receipts while respecting payer privacy and consent; specific banknotes are fungible, so attribute pooled payments using stable accounting references, not invented physical money tracing.

**Explicit phase gate from the owner:** Do **not** start coding a donation gateway, payment processor, public financial watchdog, invoices or funding dashboard now. Build it **only after Afrowave offers useful running services and there is an actual meaningful reason to request voluntary contributions**. For now record the intended behavior and return to translation/localization implementation. No service charges or donation collection are authorized by this architecture note.

## Simple public accounting: actual totals, not a tax-code itemization (2026-10-20)

**Author's settled UX decision (overrides earlier examples of showing multiple tax/FX/fee columns to everyone):** Public and donor-facing funding views must be **short, understandable and reconciled**, not a line-by-line accounting ledger. Transparency means showing the actual total paid and what remains—not confronting users with VAT breakdowns, tax rates, ledger codes and accounting components that do not change the actual purchase price.

### Default public presentation

Show the **actual gross contribution**, the **total payment/transfer cost** as one comprehensible charged amount (when any), the **net available amount**, the user-selected destination amounts (e.g. AI and servers), and later the **actual whole-price expense** (e.g. 'VPS operation: USD 6.00, all applicable purchase taxes included'), the useful service funded and the still-unused purpose-bound balance. Pooling several contributions into future-month VPS operation remains visible as reserved/unspent money, not invented current spending.

**Example, hypothetical:** gross USD 10.00; transfer fee USD 0.40; net USD 9.60; after an explicitly disclosed proportional fee treatment, AI USD 4.80, servers USD 4.80. This is an *illustration*, not a proposed mandatory fee rate or the only allocation rule. If fees are covered elsewhere, donor-designated net USD 5 + USD 5 may instead remain whole. Always show the rule and effective net allocation **before** payment.

Purchase display is **one actual charged amount**, not 'before VAT + VAT + handling + ...'. A VPS invoice with tax components is publicly summarized by its **real total paid**. If an additional independent transfer charge actually reduces funds, show that one real charge clearly, not an unexplained difference or multiple confusing tax subrows. Public expense category/service purpose may have a concise accessible description; do not falsely aggregate unrelated expenses into one item.

### Detailed internal records still required

Internally retain original payment receipts, invoices and legally necessary tax/FX details so amounts and bank/provider balances reconcile accurately to the smallest currency unit, and auditors can verify them. **Internal evidence detail and simple public reporting are distinct presentations of the same authoritative facts**, not different accounting realities. The public may open a privacy-safe supporting receipt if desired, rather than being forced through its subfields. No personally identifying donor data or payment secrets in public evidence.

**Timing:** this is an interface/accountability requirement for later. Do not implement the payment portal, finance watchdog or a ledger until an actually useful Afrowave service exists and there is a concrete reason to accept voluntary support. No payment processing, accounting or UI implementation in this documentation change.

## Bank-backed public transaction mirroring (future option, 2026-10-20)

**Owner's additional long-term preference:** if and when Afrowave begins accepting optional contributions, seek a dedicated bank account/provider offering an officially supported **public read-only API** or equivalent transparent-account feed. Publish an up-to-date independently bank-sourced view of incoming booked transactions, outgoing booked transactions and the bank-confirmed available/book balance, in addition to the previously agreed simple cost/purpose dashboard. The objective is verifiable community trust: anyone should be able to compare our public finance summaries with **actual source-bank postings** rather than relying only on an Afrowave-operated ledger.

**Never promise instantaneous posting merely because an API exists.** Display a visible bank-data `asOf`/last-successfully-synchronized timestamp, feed freshness and distinction between pending, booked/settled and reconciled items according to actual bank capabilities. Update via authenticated signed webhook if officially offered, or read-only polling with appropriate rate/cache behavior. A finance watchdog can report broken synchronization or accounting differences later, but there is no such implementation now.

**Bank mirror vs. purpose ledger:** the external bank feed certifies transfer amounts, references, directions and balance *to the extent the selected API exposes them*. It does **not** by itself prove that a particular donation is assigned USD 5 to AI and USD 5 to VPS, that AI tokens were credited, or that a VPS was actually deployed. Those facts remain in separately auditable, privacy-safe purpose-allocation, invoice and provisioned-service records. Reconcile the bank balance to our actual ledger balance and explicitly label restricted future-month reserves, prepaid provider credits, processing fees and unapplied receipts without inflating paid expenses.

### Security and human privacy are not optional

- Use a *dedicated* account for Afrowave community finances and choose a bank/product that **legitimately authorizes public disclosure** of the desired transfer data. A normal private-account PSD2/open-banking API is not automatically public and must not be treated as an unrestricted open statement.
- The public client/browser must never receive bank credentials, banking API keys, refresh/access tokens, or payment-initiation privileges. An isolated backend mirror uses **read-only least-privilege access** to the official bank feed; do not expose transfer/write methods or allow a public endpoint to initiate payments.
- The owner wants incoming/outgoing **amounts and balances publicly visible**. Check what a real public account legally and technically exposes about counterparties: names, account numbers, payment references and user-entered payment messages can contain private information. Prefer a bank transparency product with privacy-safe public fields, explicitly inform donors *before payment* what the bank itself will publish, and do not promise a local anonymizer can suppress information already published directly by the bank. Where compatible with the banking product, project a redacted public view while retaining a complete confidential audit record.
- Provide stable bank transaction IDs/deduplication, deterministic handling of reversals/chargebacks/corrections, pagination and correct multicurrency display/FX; a temporary bank API failure must never produce fictional transactions or a falsely fresh balance. Never publicly display secrets or tenant/donor personal data solely to prove transparency.

### Interface expectations

Main public view remains deliberately simple: **money in · money out · current bank balance · last bank update**; optional click-through shows the bank-confirmed dated entry, purpose attribution, actual expenditure and still-unspent amount. Avoid VAT/accounting subline clutter. A publicly accessible view should not require user login to see aggregate financial flows. The selected bank feed's own public source/verification link should be shown whenever it exists.

**Phase gate remains explicit:** this is an intended bank-account selection criterion and future design **only**. Do not choose a bank, request credentials, create the account, collect donations, build the finance watchdog, implement live mirroring, or claim current transactions now. Revisit only when functional Afrowave services create a genuine reason to accept contributions.

