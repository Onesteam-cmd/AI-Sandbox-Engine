# ADR-0072: Host recovery multi-collection-sequence-sequence checkpoint-range summary and adjacent multi-collection-sequence selection contracts

- **Status:** Accepted
- **Date:** 2026-08-06
- **Scope:** Immutable checkpoint-range summary projection and bounded adjacent multi-collection-sequence selection authorities

## Context

Commit 0071 validates one exact continuous multi-collection-sequence sequence and resolves one bounded checkpoint range crossing one or more validated multi-collection-sequence boundaries. Callers can inspect the complete immutable query authority, but Core still needs one compact projection of that exact range and one explicit advisory authority selecting a caller-sized multi-collection-sequence immediately before or after it.

The supplied source order remains authoritative. Core must not discover missing summaries, reorder them, load or store authorities, index or retain history, archive, delete, compact, paginate, diagnose, restart, schedule, supervise, wait, transport, mutate history, or execute external work.

## Decision

Introduce eight public contracts:

- `HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjectionIdKind`;
- `HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelectionIdKind`;
- `HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryStatus`;
- `HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection`;
- `HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection`;
- both explicit result contracts;
- `HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryFlow`.

`ProjectSummary` accepts one externally assigned summary identity, one exact `HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQuery`, its expected revision, and an external projection tick. It preserves the unchanged query, validation sequence, source authorities, checkpoints, crossed boundaries, indexes, optimistic revision, and monotonic time.

`SelectPreviousMultiCollectionSequence` and `SelectNextMultiCollectionSequence` accept one externally assigned selection identity, one exact summary, a positive bounded count, its expected revision, and an external selection tick. They select exact `HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection` authorities immediately adjacent to the summarized interval, preserve source order and every internal boundary, and retain the exact supersession connecting selection and range.

All created authorities remain externally identified, immutable, revisioned, bounded, synchronous, deterministic, and monotonic in external time.

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

- callers receive one compact immutable proof over an exact validated range;
- adjacent selection retains exact multi-collection-sequence summaries, internal boundaries, source authorities, endpoints, aggregate evidence, revision, and external tick;
- stale revisions, regressed ticks, oversized counts, missing adjacent summaries, short source intervals, and boundary mismatches produce explicit outcomes;
- caller-owned collections cannot mutate created authorities;
- the Core boundary remains side-effect free and deterministic.

Negative:

- callers must supply exact range and summary authorities, matching revisions, positive bounded counts, and monotonic ticks;
- malformed or unavailable adjacent evidence is rejected rather than repaired;
- Core does not discover, reorder, materialize, or paginate authorities beyond the exact supplied range and adjacent selection.
