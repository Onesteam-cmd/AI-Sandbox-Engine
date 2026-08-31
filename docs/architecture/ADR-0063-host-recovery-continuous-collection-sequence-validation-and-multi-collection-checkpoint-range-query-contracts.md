# ADR-0063: Host recovery continuous collection-sequence validation and multi-collection checkpoint-range query contracts

- **Status:** Accepted
- **Date:** 2026-08-05
- **Scope:** Immutable continuous collection-sequence validation and bounded inclusive multi-collection checkpoint-range query authorities

## Context

Commit 0062 projects one exact continuous collection-pair summary over a
validated multi-sequence checkpoint-range continuity authority and resolves one
bounded range across that pair's shared collection boundary. A host can now
supply several externally identified collection-pair summaries, but Core still
needs an explicit authority proving that the supplied order is continuous and a
read-only query authority over ranges that cross one or more validated
collection boundaries.

The input order is authoritative. Core must neither discover missing summaries
nor reorder, store, index, retain, archive, delete, compact, paginate,
diagnose, restart, schedule, supervise, wait, transport, mutate history, or
execute external work.

## Decision

Introduce eight public contracts:

- `HostRuntimeRecoveryContinuousCollectionSequenceValidationIdKind`;
- `HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryIdKind`;
- `HostRuntimeRecoveryContinuousCollectionSequenceStatus`;
- `HostRuntimeRecoveryContinuousCollectionSequenceValidation`;
- `HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery`;
- both explicit result contracts;
- `HostRuntimeRecoveryContinuousCollectionSequenceFlow`.

`ValidateSequence` accepts one through eight exact
`HostRuntimeRecoveryContinuousCollectionPairSummaryProjection` authorities,
one expected revision per authority, and an external validation tick. It copies
the caller list, preserves exact references and order, rejects duplicate
external IDs, stale revisions, regressed time, source mismatches, non-adjacent
summary or checkpoint intervals, and mismatched internal or connecting
supersessions. Successful validation stores every exact collection boundary in
chain order and advances from the maximum source revision.

`QueryRange` accepts an exact sequence authority, inclusive start and end
checkpoint IDs, its expected revision, and an external query tick. It searches
only inside the validated sequence interval, rejects missing or reversed
boundaries, requires at least one validated collection boundary, limits the
materialized range to 64 checkpoints, and stores exact checkpoints,
supersessions, crossed boundaries, incoming and outgoing supersessions, chain
indexes, and intersected collection-pair indexes.

Both authorities remain externally identified, immutable, revisioned, and
monotonic in external time. Source collection validation, source projection,
supersession chain, checkpoints, and supersessions are preserved by reference.

## Bounds

- Maximum collection-pair summaries per validation: **8**.
- Maximum checkpoints per multi-collection range: **64**.

These bounds keep validation and materialization deterministic and prevent Core
from becoming an unbounded query or storage subsystem.

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

- callers receive exact continuous collection-sequence evidence;
- optimistic revision and monotonic-time failures are explicit;
- multi-collection ranges preserve all crossed boundary authorities;
- caller-owned collections cannot mutate created authorities;
- the Core boundary remains side-effect free and deterministic.

Negative:

- callers must provide the complete ordered sequence and expected revisions;
- queries are bounded and must be split explicitly by the host when larger;
- Core does not repair gaps, overlaps, stale authorities, or source mismatches.
