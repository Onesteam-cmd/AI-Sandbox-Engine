# ADR-0064: Host recovery multi-collection checkpoint-range summary and adjacent collection-sequence selection contracts

- **Status:** Accepted
- **Date:** 2026-08-05
- **Scope:** Immutable multi-collection checkpoint-range summary projection and bounded adjacent collection-sequence selection authorities

## Context

Commit 0063 validates one exact continuous collection sequence and resolves one
bounded checkpoint range crossing one or more validated collection boundaries.
Callers can inspect the full immutable query authority, but Core still needs a
compact projection of that exact range and an explicit advisory authority for
selecting a caller-sized collection sequence immediately before or after it.

The source sequence and caller-provided direction remain authoritative. Core
must not discover, reorder, load, store, index, retain, archive, delete, compact,
paginate, diagnose, restart, schedule, supervise, wait, transport, mutate
history, or execute external work.

## Decision

Introduce eight public contracts:

- `HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionIdKind`;
- `HostRuntimeRecoveryAdjacentCollectionSequenceSelectionIdKind`;
- `HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus`;
- `HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection`;
- `HostRuntimeRecoveryAdjacentCollectionSequenceSelection`;
- both explicit result contracts;
- `HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryFlow`.

`ProjectSummary` accepts one exact
`HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery`, its expected revision,
and an external projection tick. It preserves the exact range, continuous
collection sequence, source collection, chain-summary projection, supersession
chain, endpoints, indexes, counts, crossed boundaries, incoming and outgoing
supersessions, boundary flags, and immutable evidence by reference.

`SelectPreviousCollectionSequence` and `SelectNextCollectionSequence` accept one
exact range summary, a positive collection-pair count, its expected revision,
and an external selection tick. They inspect only the source sequence, preserve
source order, and select the exact immediately adjacent collection-pair summary
authorities and their internal and connecting boundary supersessions. Missing,
short, stale, regressed-time, oversized, and boundary-mismatch outcomes are
explicit.

All created authorities remain externally identified, immutable, revisioned,
bounded, and monotonic in external time.

## Bounds

- Maximum collection-pair summaries per adjacent collection sequence: **8**.

The bound matches the continuous collection-sequence validation limit and keeps
selection deterministic without turning Core into a discovery, pagination, or
storage subsystem.

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

- callers receive a compact exact authority for one multi-collection range;
- optimistic revision and monotonic-time failures remain explicit;
- adjacent collection-sequence selection preserves exact source references and
  boundary evidence;
- caller-owned inputs cannot mutate created authorities;
- the Core boundary remains side-effect free and deterministic.

Negative:

- callers must supply the exact source authority, direction, count, revision,
  and external tick;
- selection cannot cross missing or malformed source boundaries;
- Core does not repair, discover, reorder, or paginate adjacent authorities.
