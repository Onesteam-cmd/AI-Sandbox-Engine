# ADR-0081: Host recovery adjacent multi-collection-sequence-sequence-sequence projection and multi-collection-sequence-sequence-sequence-sequence checkpoint-range continuity validation contracts

- **Status:** Accepted
- **Date:** 2026-08-06
- **Scope:** Immutable adjacent multi-collection-sequence-sequence-sequence projection and exact multi-collection-sequence-sequence-sequence-sequence checkpoint-range continuity-validation authorities

## Context

Commit 0080 projects one exact bounded multi-collection-sequence-sequence-sequence-sequence checkpoint-range
summary and selects one exact previous or next adjacent multi-collection-sequence-sequence-sequence from the
same validated source sequence. Callers can inspect the selection authority, but
Core still needs an explicit materialized projection of its exact checkpoints and
supersessions and an authority proving immediate multi-collection-sequence-sequence-sequence, checkpoint,
and supersession continuity with the summarized range.

The source summary and caller-provided selection remain authoritative. Core must
not discover, reorder, load, store, index, retain, archive, delete, compact,
paginate, diagnose, restart, schedule, supervise, wait, transport, mutate
history, or execute external work.

## Decision

Introduce eight public contracts:

- `HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionIdKind`;
- `HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationIdKind`;
- `HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus`;
- `HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection`;
- `HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation`;
- both explicit result contracts;
- `HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceFlow`.

`ProjectMultiCollectionSequenceSequenceSequence` accepts one exact
`HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection`, its expected revision,
and an external projection tick. It verifies the exact selected multi-collection-sequence-sequence-sequence
summary references, internal and adjacent boundary supersessions, checkpoint
interval, and incoming/outgoing supersessions against the immutable source
sequence and chain before materializing exact read-only checkpoint and
supersession arrays.

`ValidateContinuity` accepts one exact
`HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection`,
one exact projected adjacent multi-collection-sequence-sequence-sequence, both expected revisions, and an
external validation tick. It requires both authorities to share the same source
summary, multi-collection-sequence-sequence-sequence intervals and checkpoint intervals to be immediately
adjacent, and the same exact connecting supersession with matching endpoint
checkpoints.

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

- callers receive an exact immutable projection of a selected adjacent
  multi-collection-sequence-sequence-sequence;
- projection rejects stale, regressed-time, source-reference, boundary,
  checkpoint, and supersession mismatches explicitly;
- continuity validation preserves the exact range summary, projected
  multi-collection-sequence-sequence-sequence, selection, connecting supersession, endpoint checkpoints,
  external tick, and optimistic revision;
- caller-owned collections cannot mutate created authorities;
- the Core boundary remains side-effect free and deterministic.

Negative:

- callers must supply exact source authorities, revisions, and external ticks;
- malformed or non-adjacent evidence is rejected rather than repaired;
- Core does not discover, reorder, materialize, or paginate adjacent authorities
  beyond the exact supplied selection.
