# ADR-0070: Host recovery continuous multi-collection-sequence summary projection and cross-multi-collection checkpoint-range query contracts

- **Status:** Accepted
- **Date:** 2026-08-06
- **Scope:** Immutable continuous multi-collection-sequence summary projection and bounded cross-multi-collection checkpoint-range query authorities

## Context

Commit 0069 materializes one exact adjacent multi-collection and proves immediate
multi-collection, checkpoint, and supersession continuity with one summarized
multi-collection-sequence checkpoint range. Callers can inspect both authorities,
but Core still needs one compact authority describing their resulting continuous
multi-collection-sequence interval and one bounded query authority for an
inclusive checkpoint range that crosses the shared multi-collection boundary.

The source continuity authority remains authoritative. Core must not discover,
reorder, load, store, index, retain, archive, delete, compact, paginate, diagnose,
restart, schedule, supervise, wait, transport, mutate history, or execute external
work.

## Decision

Introduce eight public contracts:

- `HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjectionIdKind`;
- `HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryIdKind`;
- `HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus`;
- `HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection`;
- `HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery`;
- both explicit result contracts;
- `HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryFlow`.

`ProjectSummary` accepts one exact
`HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation`,
its expected revision, and an external projection tick. It verifies the unchanged
range summary and projected adjacent multi-collection authority, immediate
multi-collection-summary and checkpoint adjacency, one exact connecting
supersession, and matching endpoint checkpoints before creating the compact
summary.

`QueryRange` accepts one exact continuous multi-collection-sequence summary,
exact start and end checkpoint identities, the expected summary revision, and an
external query tick. It resolves an inclusive source-chain range only when that
range crosses the validated multi-collection boundary and does not exceed the
existing checkpoint bound.

All created authorities remain externally identified, immutable, revisioned,
bounded, synchronous, deterministic, and monotonic in external time.

## Rejected responsibilities

This increment does not perform:

- discovery or reordering;
- loading, storage, indexing, retention, archival, deletion, or compaction;
- pagination or diagnostics;
- restart, scheduling, supervision, waiting, transport, or execution;
- hidden wall-clock reads, generated IDs, retries, tasks, or providers;
- history mutation.

Those responsibilities remain explicit host or adapter concerns outside Core.

## Consequences

Positive:

- callers receive one compact immutable summary over a validated continuous
  multi-collection-sequence interval;
- bounded queries retain exact checkpoint and supersession authorities across the
  multi-collection boundary;
- stale revisions, regressed ticks, source mismatches, non-contiguous indexes,
  boundary mismatches, missing endpoints, invalid order, oversized ranges, and
  missing connecting supersessions produce explicit outcomes;
- caller-owned collections cannot mutate created authorities;
- the Core boundary remains side-effect free and deterministic.

Negative:

- callers must supply exact source authorities, revisions, ticks, and checkpoint
  identities;
- malformed or non-crossing evidence is rejected rather than repaired;
- Core does not discover, reorder, materialize, or paginate authorities beyond
  the exact supplied bounded range.
