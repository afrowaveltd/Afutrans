# Target-Only Localization Invalidation and Missing-Target Reconciliation

**Status: new UnderstandMe API implemented in Toolbox, 2026-10-20; Afutrans scanning/automatic regeneration remains future work.**

## Problem and selected repair strategy

If source template `I have {count} kids` is valid, but its `cs` translation is `Mám děti` (missing `{count}`) or `Mám {count} a {count} dětí` (extra `{count}`), the target is **invalid**. Do **not** delete the English/default source key or any German/other-language translation to trigger a global regeneration.

Instead **remove only** the target locale entry (`cs[key]`) using `ITextLocalizer.InvalidateTranslation("cs", key)` (or the default automatic invalidation after runtime placeholder validation). The C# API and optional writable-store interface were added in Toolbox [commit `1a08085`](https://github.com/afrowaveltd/Toolbox/commit/1a08085fc5619e03938fb76afd25c900e9717fe9), with a successful [.NET GitHub Actions run](https://github.com/afrowaveltd/Toolbox/actions/runs/39410576675).

The localizer immediately falls back to a **valid parent culture or source template**, preserving the runtime value `count=3`. It does not require a live translation backend or wait for network access. Read-only stores and deployments configured with `AutoInvalidateInvalidTargetTranslations=false` keep safe fallback without automatic deletions. Target deletion is conditional on the observed value still being present, so a more recent correction is not overwritten by that operation. Source deletion is always prohibited through this method.

## Afutrans scanner must compare target completeness even without source changes

The existing design for source scanning captures source snapshots/revisions and schedules work for newly discovered or changed source phrases. **That alone is insufficient here:** invalidating `cs[key]` leaves the source dictionary **unchanged**. A target that went missing after the last checkpoint must still be discovered and recreated.

For each authorized Translation Target, scan its configured **source catalog plus selected target-locale dictionaries** and detect:
- `MissingTranslation(sourceKey, targetCulture)`: authoritative source entry exists but target entry is absent.
- An existing but placeholder-invalid target value (when direct scanner validation is enabled), handled as a focused target repair, not a source deletion.
- Source revision changed under symbolic keys; any translation referring to the old source revision must be invalidated/retranslated under separate source-revision logic.
- Already valid translations in any culture remain untouched, including reviewed manual overrides; optional provenance/audit retains prior invalid text if configured.

Create a **single target-locale job** through the standard Afutrans Orchestrator/QueueMaster and validate the replacement template with the same placeholder-name/occurrence check before an authorized revision-safe publish. Prefer idempotency key including `TargetId + sourceKey + sourceRevision + targetCulture`; avoid duplicate work from push event and polling. A repeated invalid machine result must not cause an infinite delete/retranslate storm: keep per-key attempt state/backoff and escalate to authorized review or a suitable alternative provider according to policy.

## Notification policy and no-event mode

- **With notifications:** a durable successful `cs[key]` invalidation may emit a scoped `TranslationInvalidated` or dictionary change event (names illustrative). It can trigger prompt targeted rescan/retranslation. The event does not replace durable state or prove repair completion.
- **Without notifications:** the periodic Afutrans target scanner still finds missing target keys independently of source-delta checks. An optional diagnostic/incident can be logged, but it must not block a localizer or silently create network dependencies.
- Both paths may be enabled, and must **deduplicate** against the same logical job. File/SQL/remote write permissions, tenant identity, culture and source revision must be checked before deleting or replacing target data.

## Optional immediate rendered-text rescue remains separate

An application may later choose an **async** realtime translation of the already-substituted source `I have 3 kids` for one display only; do not store that result under the reusable `I have {count} kids` dictionary key. Source fallback remains correct if no translator is available. Only send personal runtime values to explicitly permitted providers under a target's privacy policy and deadline.

## Acceptance tests for Afutrans once scanner exists

1. English source and all target dictionaries unchanged except `cs[key]` removed: next scheduled target completeness scan still queues **only** the missing `cs` job.
2. Target locale `cs-CZ` invalid but `cs` parent valid: parent fallback works, and `cs-CZ` regeneration does not rewrite `cs` or English.
3. Push and polling both see the same missing entry: enqueue once; scanner checkpoint advances only after successful work admission.
4. Machine returns missing/duplicate/renamed placeholder again: never publish an invalid value, and apply bounded retry/backoff instead of infinite churn.
5. Read-only catalog and no-event mode: localizer remains useful, reports only intended diagnostics, never deletes authoritative source text.
6. A concurrent target edit supplies a valid translation: stale invalidation cannot delete the newer value; do not overwrite human corrections.
7. Live rendered-text rescue is one-shot and privacy-scoped; it does not create a reusable broken target template.

**Implementation boundary:** currently only the Toolbox localizer/store invalidation and unit tests exist. This document does **not** implement the Afutrans scanner, event channel, incident pipeline, multi-provider translation, JSON/SQL synchronization, QueueMaster or any online rescue.
