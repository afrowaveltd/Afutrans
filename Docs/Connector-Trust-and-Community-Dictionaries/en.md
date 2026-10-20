# Owner-selected connectors, trust boundaries, and opt-in community dictionaries

**Status: 2026-10-20 product requirements and proposed architecture, not implemented.** This document refines [Registered Service Integration](../Registered-Service-Integration/en.md) and the common [shared dictionary design](https://github.com/afrowaveltd/Toolbox/blob/master/UnderstandMe/Docs/Localization/Shared-Translation-Reuse-and-Placeholder-Signatures/en.md).

## Guiding principles

1. The **owner** of a Translation Target selects compatible protocols and storage backends, including a deliberate insecure/test-only mode. Afutrans should recommend secure methods and visibly disclose risks, **not prohibit owners from using an insecure protocol for their own noncritical resources**.
2. The right to risk the **owner's** resources is **not** the right to endanger the hosted translation service, Id, other tenants, credentials, shared compute or platform identity. Cross-tenant containment and Id's own credential/token transport security are mandatory infrastructure invariants.
3. The user-facing service must clearly disclose that Afutrans **can process and technically access the content** of dictionaries to which the owner grants it access. Access for translation or diagnostics does **not** imply a right to publish, sell, or share those contents.
4. A community translation corpus should **reuse as much approved work as possible**, rather than duplicating the same entries among many small dictionaries. Publishing is a separate **explicit, revocable-for-future-distribution opt-in** with provenance, contextual checks and owners retaining their private/local overrides.

## Configurable connector protocols

| Example connection | Owner option and warning | Host enforcement boundary |
| --- | --- | --- |
| Local filesystem | Scoped approved root; least-privilege read/write | No path traversal, symlink escape or reads outside the authorized root |
| SFTP/FTPS/HTTPS | Recommended encrypted network transport | Verify remote identity according to declared trust policy; scope credentials/egress |
| Classic plaintext FTP | **Supported as an explicit opt-in**; clear warning that usernames, passwords and dictionary content can be intercepted or changed in transit; do not claim confidentiality or integrity | Run in an isolated unprivileged connector process/container with constrained filesystem/network/capability; never pass Id or shared-service secrets |
| Unencrypted HTTP file/API test endpoint | **Explicit owner-controlled, clearly labeled insecure/test** mode may be supported for non-sensitive target resources | Never allow HTTP to carry Id authorization codes, Id tokens, or centrally managed login credentials to non-loopback public endpoints; no automatic upgrade to privileged control plane |
| SQL and other databases | Per-target connection; read-only unless explicit write grant | Principle of least privilege, secret references, tenant boundaries, validated queries/paths and optimistic concurrency |

Secure transports are **recommended defaults** rather than a universal connector ban policy. If source protocols offer no transport confidentiality or integrity, translated results may be manipulated; document this and avoid promises of total security. Host operators still control network egress, resource isolation, credential vault policies, allowed public endpoint access and whether optional insecure connectors are available in their own deployment. A shared service need not accept arbitrary third-party network destinations.

### Protect the hosted platform from insecure target sources

- Connector workers must not run with Id, platform, database-superuser or other tenants' filesystem credentials. Store each target credential separately and give the worker only that target's authorized secret reference; scrub secret material from logs/events.
- Do not let an FTP/HTTP/SQL connector become an **SSRF/internal-network proxy**. Apply network destination policy, DNS rebinding protection, reserved/internal address restrictions where appropriate, strict time/byte limits and safe redirect handling. Operators may explicitly authorize legitimate internal host access in their own trusted deployment.
- Enforce per-target CPU/RAM/network/disk/job quotas and isolate file parsers. HTML and translated content must be treated as **untrusted data** (no script execution in previews; no unsafe deserialization or automatic process spawning).
- Validate dictionary syntax, locales, placeholders and output revision before acceptance; honor source application permissions. All writes use atomic/transactional semantics where the backend permits and never overwrite accepted manual edits silently.
- An external compromise can still affect availability through resource consumption, a bug in the connector or contaminated source data; isolation reduces blast radius, it is not an absolute guarantee. Test realistic hostile inputs and connector crashes.
- Id login/SSO is a separately protected trust domain. The owner's decision to publish plain HTTP/HTML is their right; Id's token/authorization transport remains secure. OAuth 2.0 Security BCP RFC 9700 prohibits unencrypted authorization responses except native-app loopback redirects: https://www.rfc-editor.org/rfc/rfc9700.

## Transparency and deliberate sharing choice

Afutrans must disclose in target setup and its operator privacy policy **what it needs to read and why**. Suggested product-level wording (subject to localization/editorial review):

> Afutrans reads the translation dictionaries and related metadata that you connect so it can process, verify, update or export translations. The service operator may be technically able to access this content. Your dictionary is private by default. You can voluntarily share selected phrases or an eligible dictionary with the Afrowave community. Sharing does not make your database credentials public.

Proposed independent switches/actions:

- `DictionaryVisibility = Private` (default): only the authorized owner/application/project may access and use the dictionary; private entries are never automatically indexed for cross-tenant reuse.
- `ShareSelectedEntries`: owner reviews and publishes individual **eligible** source+translation pairs; support targeted omissions/exclusions.
- `ShareEligibleDictionary`: owner voluntarily opts in to publishing eligible phrases from an entire catalog with a preview of exactly what becomes available, language pair and content filters. New entries/edits should follow a clearly stated ongoing-publication setting or require explicit confirmation, not a surprise implicit share.
- `ContributeWithoutPublicSourceFile` (possible optional mode): contribute approved phrase records to the shared index without exposing raw file paths, tenant DB schema, target IDs, source dictionary layout or other proprietary metadata.
- Owner can stop future contributions, unpublish their contributions from the managed index and change selection; already downloaded/copied published translations **cannot reliably be recalled**, which must be disclosed before publication.

The app owner must have the rights to publish contributed content; do not publish connected private messages, user-generated personal details, tokens/passwords, URLs with secrets, restricted documents or a client's app-specific proprietary data by default, even if ordinary UI text sharing is enabled. Human review/configurable filters can help. Use explicit review/permissions and report invalid or infringing entries.

## One huge logical translation memory; independent owner control

- Shared **logical** search/index can span many authorized data stores or a central normalized corpus. No requirement to merge all owners into one physical file, table or giant unversioned JSON.
- Each public entry carries **source text/key convention**, normalized source culture and target locale (include region/script), validated target text, placeholder canonical signature/mapping and multiplicity, context/domain/grammar/case distinctions, provenance/source revision, contributor/owner access scope, publication/license rights, confidence/review status and update/withdrawal state.
- Reuse order: owning application's exact approved/manual override; its own validated dictionary; authorized compatible shared candidates; optional paid/free machine translation as configured. A community candidate is a **suggestion until validated** for target language, placeholders, local context and policy; ambiguous candidates are not silently treated as universal truth.
- Example ambiguity: `My baby` in English and `my baby` in Czech (colloquial 'we old ladies') can have opposite meanings; `US` vs `us` differs by capitalization. The correct source language and semantics must be part of lookup, not simply the letters.
- `FindInAnyDictionary` can search across only **published or otherwise explicitly authorized** sources, with index/locale/context/revision isolation. Prefer reuse over retranslating duplicates. Once valid, an owner may choose to copy a translation into a private project dictionary without acquiring ownership of the original source dictionary.
- Shared phrase reuse should not silently overwrite manually approved local translations; corrections, credits, provenance and moderation of public entries require their own workflow and auditable revisions. Contributor withdrawal removes managed index availability for future consumers, but cannot erase copies previously lawfully downloaded.
- Owners choose whether their translations are licensed for community reuse, including attribution and downstream rights where applicable. Service visibility (`Public API`), availability of a dictionary to its owner, and publishing translation content to a community corpus are **three different permissions**.

## Future tests

1. Owner B selects classic FTP to a throwaway test instance, confirms a prominent insecurity warning, and can read its permitted dictionary; an attacker-controlled source cannot reach Id endpoints, other owners' files or secrets.
2. An ordinary public HTTP demo app can be listed in Id without being granted an insecure Id authorization-code/token redirect; secure SSO uses an HTTPS callback or a separately supported native loopback flow.
3. Private target A is never found by B via `FindInAnyDictionary`, even when Afutrans processes A's translation requests.
4. A publishes selected phrase pairs and B reuses those compatible variants; A's private metadata/file paths and connector credentials remain hidden.
5. Publishing an entire eligible catalog shows a preview, filters sensitive/restricted content and makes the ongoing update setting explicit; withdrawal prevents further index discovery and makes copy-retention limits clear.
6. Same text with different source culture, context, case or placeholder structure does not produce unsafe reuse; local manual approvals prevail.
7. REST and business SignalR entry points apply identical per-client target/read/write/sharing permissions, quota and audit.

**Implementation:** these modes, connector isolation, opt-in publication, public corpus and corpus search are design work only; no running public connector/API or test suite is implied.
