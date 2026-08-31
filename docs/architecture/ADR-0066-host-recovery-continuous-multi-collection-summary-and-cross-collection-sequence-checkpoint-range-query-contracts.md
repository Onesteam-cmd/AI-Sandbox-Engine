# ADR-0066: Host recovery continuous multi-collection summary projection and cross-collection-sequence checkpoint-range query contracts

- **Status:** Accepted
- **Date:** 2026-08-05
- **Scope:** Immutable continuous multi-collection summary projection and bounded cross-collection-sequence checkpoint-range query authorities

## Context

Commit 0065 materializes one exact adjacent collection sequence and proves immediate
collection-pair, checkpoint, and supersession continuity with one summarized
multi-collection checkpoint range. Callers can inspect both authorities, but Core
still needs one compact authority describing their resulting continuous
multi-collection interval and one bounded query authority for an inclusive
checkpoint range that crosses the shared collection-sequence boundary.

The source continuity authority remains authoritative. Core must not discover,
reorder, load, store, index, retain, archive, delete, compact, paginate, diagnose,
restart, schedule, supervise, wait, transport, mutate history, or execute external
work.

## Decision

Introduce eight public contracts:

- `HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionIdKind`;
- `HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryIdKind`;
- `HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus`;
- `HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection`;
- `HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery`;
- both explicit result contracts;
- `HostRuntimeRecoveryContinuousMultiCollectionSummaryFlow`.

`ProjectSummary` accepts one exact
`HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation`, its
expected revision, and an external projection tick. It verifies the unchanged
range summary and adjacent collection-sequence authority, immediate
collection-pair and checkpoint adjacency, one exact connecting supersession, and
matching endpoint checkpoints before creating the compact summary.

`QueryRange` accepts one exact continuous multi-collection summary, exact start
and end checkpoint identities, the expected summary revision, and an external
query tick. It resolves an inclusive source-chain range only when that range
crosses the validated collection-sequence boundary and does not exceed the
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
  multi-collection interval;
- bounded queries retain exact checkpoint and supersession authorities across the
  collection-sequence boundary;
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
