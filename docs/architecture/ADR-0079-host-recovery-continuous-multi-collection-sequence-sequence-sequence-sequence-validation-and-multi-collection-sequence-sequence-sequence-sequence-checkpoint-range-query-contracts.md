# ADR-0079: Host recovery continuous multi-collection-sequence-sequence-sequence sequence validation and multi-collection-sequence-sequence-sequence-sequence checkpoint-range query contracts

- **Status:** Accepted
- **Date:** 2026-08-06
- **Scope:** Immutable continuous multi-collection-sequence-sequence-sequence sequence validation and bounded inclusive checkpoint-range query authorities

## Context

Commit 0074 projects one exact continuous multi-collection-sequence-sequence-sequence
summary over a validated continuity authority and resolves one bounded range
across that summary's multi-collection-sequence boundary. A host can now supply
several exact multi-collection-sequence-sequence-sequence summaries, but Core still
needs an explicit authority proving that their caller-supplied order is
continuous and one bounded query authority over ranges crossing one or more
validated multi-collection-sequence-sequence-sequence boundaries.

The supplied order remains authoritative. Core must not discover missing
summaries, reorder them, load or store authorities, index or retain history,
archive, delete, compact, paginate, diagnose, restart, schedule, supervise,
wait, transport, mutate history, or execute external work.

## Decision

Introduce eight public contracts:

- `HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidationIdKind`;
- `HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryIdKind`;
- `HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus`;
- `HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation`;
- `HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQuery`;
- both explicit result contracts;
- `HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceFlow`.

`ValidateSequence` accepts one externally assigned validation identity, one
caller-supplied ordered collection of one to eight exact
`HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection`
authorities, their matching optimistic revisions, and an external validation
tick. It snapshots the supplied collection, rejects stale revisions or
regressed time, requires unique summary identities and exact shared source
sequence, collection, projection, and chain authorities, verifies each
summary's internal connecting supersession, and proves immediate
multi-collection-sequence-summary and checkpoint continuity between
consecutive summaries.

`QueryRange` accepts one exact continuous
multi-collection-sequence-sequence-sequence-sequence validation, exact start and end
checkpoint identities, its expected revision, and an external query tick. It
resolves one inclusive source-chain range only when the range crosses at least
one validated multi-collection-sequence-sequence-sequence boundary, remains inside the
existing checkpoint bound, and preserves every crossed boundary supersession.

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

- callers receive one immutable proof over an ordered bounded sequence of exact
  continuous multi-collection-sequence-sequence-sequence summaries;
- checkpoint-range queries retain exact checkpoints, supersessions, source
  authorities, and every crossed multi-collection-sequence-sequence-sequence boundary;
- stale revisions, regressed ticks, duplicate identities, source mismatches,
  malformed internal boundaries, non-contiguous intervals, missing endpoints,
  invalid order, oversized ranges, and boundary mismatches produce explicit
  outcomes;
- caller-owned collections cannot mutate created authorities;
- the Core boundary remains side-effect free and deterministic.

Negative:

- callers must supply exact summaries, matching revisions, checkpoint
  identities, and monotonic ticks;
- malformed or discontinuous evidence is rejected rather than repaired;
- Core does not discover, reorder, materialize, or paginate authorities beyond
  the exact supplied bounded sequence and range.
