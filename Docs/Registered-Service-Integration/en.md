# Afutrans as a registered Afrowave service application

**Status: architecture/design requirements (2026-10-20); not implemented.** Afutrans must remain usable independently of Id, including local/offline use and manual-only translation. Id integration is an **optional** hosted deployment adapter, not a core library dependency.

## Product intent

Afrowave Id is the first-party identity provider and application/service registry for the community. A person signs up **once in Id**; brands and their own registered applications have independent internal roles and admission policies. The central Id application is itself registered, uniquely trusted for platform authentication/oversight (via protected `IsAuthenticator`); it is not authorized to edit unrelated application roles or business content.

Afutrans can likewise be a **registered service application** owned by Afrowave or another provider. Id uses Afutrans to localize its own UI, while other registered community applications may discover and, **if authorized**, consume its translation services. The same registry should support future service applications without hard-coded Afutrans logic.

## Independent registry dimensions

Proposed concepts/field names are illustrative, not frozen C# APIs:

| Dimension | Example | Rule |
| --- | --- | --- |
| Application identity | Id, Afutrans, other service/client app | One registered application ID and owning Brand per app |
| Application capabilities/type | `OffersTranslation`, other capabilities; service/ordinary app presentation | Describes supported functions and endpoint contracts, **not privileges** |
| Discovery visibility | unlisted / listed publicly / listed to authenticated users | Determines who can **find** a service, not who may use its API |
| Service exposure | disabled / internal / opted-in external REST/SignalR | Network listeners/API exposure require explicit operator choice |
| Service access policy | owner-only / selected apps / registered apps / deliberate anonymous-public profile | Every accepted request goes through effective client policy; public **does not mean unlimited or free** |
| Entitlements | scopes, enabled provider families, quotas, rate/concurrency caps, budget | Intersection of global, provider-account, service, caller/app and translation-target policy |

`IsAuthenticator` is reserved to the single trusted Id platform identity; Afutrans is a normal registered service, never an authenticator just because it supports API clients. Id may suspend/revoke platform access to an app but cannot edit its internal role definitions.

## Registration and consumption workflow

1. The authorized operator registers the Afutrans **service application** and its owner/brand in Id, declares optional service capabilities, REST API and/or business SignalR endpoint metadata, visibility and provider-defined access policy. Do not assume a third-party service is automatically trusted because it appears in a public directory.
2. A developer owns Brand B and registers application B1 through Id. B1 may be public, private or invitation-only; these admission settings are **independent** of Afutrans service access.
3. B1's authorized owner discovers Afutrans and requests/subscribes to an allowed translation capability. Id/registry records the application-to-service relationship and authorized scopes; Afutrans also resolves its own client plan/capacity policy. The Id directory is not the owner of B1's dictionary content.
4. B1's owner creates a **Translation Target** in Afutrans: source/default language, required target languages, key strategy, dictionary format, connector access and allowed write-back, synchronization mode (periodic scan, push, manual), privacy and translation-provider policy. All are configurable per target.
5. An authorized connector reads the selected source version/snapshot. UnderstandMe supplies generic parsing, placeholder integrity and localization contracts; Afutrans orchestrates queued translation, revision-safe validation and persistence; Id supplies identity, registry and optional authorization integration only.
6. Completed approved translation updates are committed to the target's authorized destination with source-revision/ETag conflict checks and preservation of manual overrides. B1 may be notified through a **business** SignalR Hub, a callback or polling; its localizer activates a validated snapshot without depending on Afutrans for every UI string.
7. Revocation/suspension must stop **future authorized reads, writes and dispatches**, handle pending jobs according to policy and keep already-owned dictionaries intact. Resumption does not silently elevate scopes. A target may be read-only and receive exported results rather than granting Afutrans write permissions.

Illustrative application-to-service connection (not an implemented public API):

```text
Id (accounts + application/service registry + access grants)
 |
 +-- Id app (registered with reserved IsAuthenticator; consumes localization)
 +-- Afutrans (registered service; optional public directory visibility)
 |     +-- business REST / SignalR (optional; policy protected)
 |     +-- translation targets + connector credentials (Afutrans/owner side)
 |     +-- UnderstandMe workers / validation / offline localizer
 +-- Brand B
       +-- App B1 -- authorized service grant --> Afutrans target B1
       +-- App B2 -- independent roles/grants --> optional own target
```

## Connector and credential boundaries

- **Folders/files:** use a configured authorized local root with path containment and permissions, or a remote connection via **SFTP** (SSH) or suitably configured FTPS/HTTPS. Plaintext legacy FTP must not carry dictionary contents or credentials over an untrusted network.
- **SQL/other databases:** support capability-scoped read or read/write DataAccess connectors, configured connection-string reference and provider-specific settings. Prefer a remote **application-controlled agent/push endpoint** when the database need not be publicly reachable. Never use an entire DBA/admin connection string merely to sync localization.
- A developer may enter connector details in the authorized Afutrans configuration UI or their own application-side integration; **Id should hold only service registration/grants and opaque references**, not collect raw FTP passwords, database connection strings or contents just because it is the identity provider.
- Secrets remain encrypted or in an appropriate secrets manager with least-privilege read rights, rotation/revocation and no exposure in URL parameters, page HTML, logs, audit exports, SignalR events or translation model prompts. Test redaction.
- Configure source language **per Translation Target**, not globally as English. Target languages, locale aliases, fallback modes, file naming, placeholders, app/target access and snapshot revision are independent.
- Write-back requires an explicit permission and validated result; read-only connectors must still work. Connector-level failure must not overwrite the last known-good dictionary. Use an explicit opt-in sharing policy before sharing reusable phrases across brands/tenants.

## Cookies, SSO, application identities and service API authorization

- User-intended **cookie router** means **the target application owns its own first-party session cookie**. Id may retain its own essential cookie for its first-party login session, but must not deliver global tracking cookies or expose Id cookies to every application's domain.
- Cross-app login is a first-party Id SSO flow; a standards-based authorization code + PKCE / OpenID Connect-style protocol is a suitable future option. The browser redirects to Id for interactive authentication, then the target application issues **its own app-session cookie** from the verified result. No third-party login providers, external social sign-in or shared wildcard-domain super-cookie are prerequisites.
- Service-to-service translation requests use validated client/application credentials or audience-restricted delegated access, with short lifetime, scoped permission, revocation and rotation where appropriate. A user browser cookie is **not** a generic authentication key for Afutrans REST/SignalR or connector access.
- Every business REST/SignalR endpoint enforces the **same** application/tenant/target authority and `ClientPolicy` via common request handlers; mere possession of an app ID or GUID does not establish identity or spending permission. Keep business Hubs distinct from the outbound-only `/SystemMonitor` Hub.
- Id uses an application role/tenant scope to decide application membership and first-party login; it **does not modify** other apps' roles because Afutrans is a shared platform service.

## Privacy, reliability and no mandatory service availability

- Use only necessary identity/consent metadata; no third-party analytics, advertising, tracking cookies or automatic pooling of private translation content. Logs and audit records should be minimized, access-scoped and retention-limited. Privacy class and storage persistence are different settings.
- Publishing Afutrans as **listed** or even offering a broadly accessible free/public plan does not make its clients' dictionaries, secret connection info, chat history or human review tasks publicly readable.
- **Bootstrap dependency must be acyclic:** Id can install, present login and a complete **locally resolved language selector**, and operate without Afutrans online. Afutrans can operate offline/standalone without Id. Online Id authorization/registry integration is optional for an Afutrans deployment; temporary Id unavailability should produce an explicit bounded result and allow already authorized local/offline operation only according to configured token lifetime/cache policy.
- Request/job dispatch, event delivery, connector access and writer transactions have distinct failure and restart semantics; durable checkpoint and version checks make retries idempotent, prevent translation loops and protect last-approved human edits.

## Proposed integration adapters (not implemented)

- `IApplicationRegistryClient` or equivalent: optional Id-backed service discovery, app identity, scopes/grants/status (read-only view from the Afutrans host).
- `IServiceClientAuthorizer` or equivalent: verifies Id-issued or deployment-local application claims and resolves the Afutrans trusted `ClientContext` without trusting caller-supplied body fields.
- `ITranslationTargetManager`: creates/updates owner-approved target configuration, separate from Id brand/application enrollment; data access, provider policy and schedule are composed from existing generic capabilities.
- `IDictionarySource`/`IDataAccessor`, `IDictionaryChangePublisher`: storage-specific provider capabilities, independently replaceable; no fixed SQL, folder or SFTP coupling.
- `IServiceSubscriptionManager` or Id-side contract: optional app-to-service grants, revocation and visible subscriptions. Directory discovery and entitlement remain distinct from authorizing each Afutrans request.

Names are placeholders, not additional compulsory projects or NuGet dependencies. Prefer the author's familiar `I...Manager`/`I...Service`, `...Async`, DI and `Result`/`Response` conventions while keeping low-level read/write interfaces small.

## Future acceptance tests

1. Afutrans is registered as publicly **discoverable**, B1 can find it but cannot call endpoints or read B2 targets until authorized; B2 can have an entirely different plan and provider allowance.
2. Id's own UI localizes from validated local resources when Afutrans is offline; existing sessions and Id bootstrap do not depend on Afutrans readiness.
3. B1 selects `sw` as source and `cs` + `fr` targets, authorizes a read-only SFTP source, and receives validated exported translations without write-back. Explicit separate authorization allows limited write-back.
4. B1 configures a SQL target by an opaque secret reference; Id registry cannot retrieve the raw connection string, a different brand cannot read it, and no secret appears in an error/trace/event.
5. Authorized B1 REST and business SignalR requests have the **same** effective per-target access and quota results. Anonymous callers cannot impersonate registered apps by supplying their ID.
6. Id suspends B1: Afutrans refuses new B1 access after authorization refresh/revocation policy, without deleting B1 dictionaries or altering roles in B1.
7. Redirect flow creates an app-owned cookie; Id cookie remains scoped to Id origin; no third-party tracking cookie is issued.
8. Import/write-back conflicts, revoked grants, blocked provider accounts, retries and disconnected SignalR clients preserve last known-good dictionaries and reviewed changes.

**Implementation status:** architectural documentation only. No Id registry/service-discovery connector, SFTP/SQL credential UI, public Afutrans business REST/SignalR, cookie router or subscription workflow has been implemented by this decision. The existing Toolbox `UnderstandMe` offline localizer and editable language JSON catalog are independent groundwork, not proof of these integration scenarios.

