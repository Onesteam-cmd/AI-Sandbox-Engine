# ADR-0068: Host recovery multi-collection-sequence checkpoint-range summary projection and adjacent multi-collection selection contracts

- **Status:** Accepted
- **Date:** 2026-08-05
- **Scope:** Immutable compact range summaries and bounded adjacent multi-collection selection authorities

## Context

Commit 0067 validates one exact ordered sequence of continuous multi-collection
summaries and resolves bounded checkpoint ranges crossing one or more exact
collection-sequence boundaries. Hosts now need a compact immutable projection of
one such range and an advisory authority selecting exact multi-collection
summaries immediately before or after it.

The caller-supplied validation order remains authoritative. Core must not discover
missing summaries, reorder them, load or store authorities, index or retain
history, archive, delete, compact, paginate, diagnose, restart, schedule,
supervise, wait, transport, mutate history, or execute external work.

## Decision

Introduce eight public contracts:

- `HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionIdKind`;
- `HostRuntimeRecoveryAdjacentMultiCollectionSelectionIdKind`;
- `HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus`;
- `HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection`;
- `HostRuntimeRecoveryAdjacentMultiCollectionSelection`;
- both explicit result contracts;
- `HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryFlow`.

`ProjectSummary` accepts one externally assigned summary identity, one exact
`HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery`, its expected
revision, and an external projection tick. It preserves the exact range, source
sequence, source collection, source projection, chain, checkpoints, crossed
boundaries, indexes, aggregate evidence, and revision.

`SelectPreviousMultiCollection` and `SelectNextMultiCollection` accept one exact
summary, a positive bounded multi-collection count, its expected revision, and an
external selection tick. They select the exact immediately adjacent
`HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection` authorities in
source order and retain every internal boundary plus the exact supersession
connecting the selection to the summarized range.

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

- callers receive one compact immutable projection over an exact bounded range;
- adjacent selections retain exact multi-collection summaries and boundary
  supersessions in source order;
- stale revisions, regressed ticks, oversized counts, missing adjacent evidence,
  short intervals, and boundary mismatches produce explicit outcomes;
- caller-owned collections cannot mutate created authorities;
- the Core boundary remains side-effect free and deterministic.

Negative:

- callers must supply exact authorities, matching revisions, positive counts, and
  monotonic ticks;
- malformed or insufficient adjacent evidence is rejected rather than repaired;
- Core does not discover, reorder, materialize, or paginate beyond the exact
  supplied sequence and bounded selection.
