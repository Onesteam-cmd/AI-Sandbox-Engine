# ADR-0050: Host Recovery Chain-Summary Projection and Checkpoint-Lineage Query Contracts

## Status

Accepted.

## Context

Commit 0049 introduced bounded validation of caller-supplied checkpoint-supersession
chains and exact latest-checkpoint selection. Callers still need a compact immutable
view of one validated chain and a deterministic way to resolve one checkpoint together
with its immediate incoming and outgoing supersession lineage.

Core must expose these advisory contracts without discovering history, loading or
storing records, mutating the chain, creating an index, archiving, deleting, retaining,
compacting, diagnosing, scheduling, supervising, waiting, restarting, transporting, or
executing.

## Decision

Add immutable externally identified revisioned
`HostRuntimeRecoveryChainSummaryProjection<TRequest, TState, TCompletion>` authority.

Projection accepts one exact validated supersession chain, an optimistic chain revision,
and an external monotonic projection tick. It preserves the exact chain as evidence and
exposes compact root/latest checkpoint, bounded count, runtime, composition, queue,
clock, World, version, simulation, capture, and supersession-time facts.

Add immutable externally identified revisioned
`HostRuntimeRecoveryCheckpointLineageQuery<TRequest, TState, TCompletion>` authority.

A lineage query accepts one exact chain-summary projection, an optimistic projection
revision, one exact checkpoint identity, and an external monotonic query tick. It scans
only the already bounded validated chain and returns the exact checkpoint authority,
its zero-based checkpoint index, and its immediate incoming and outgoing supersession
authorities when present.

Explicit outcomes cover stale chain or projection revisions, regressed projection or
query time, and a checkpoint identity that is not represented by the chain.

## Consequences

- Query cost is bounded by the existing maximum of 256 supersession edges.
- Root, intermediate, and latest checkpoint positions are represented without hidden
  discovery or reordering.
- Source chain, supersession, completed-cycle, and checkpoint authorities remain
  unchanged evidence.
- IDs and time remain externally supplied.
- Persistence, indexing, history discovery, archival, deletion, retention, compaction,
  diagnostics, scheduling, supervision, waiting, restart, transport, and execution
  remain outside Core.

## Rejected alternatives

### Build and retain a chain index inside Core

Rejected. Index ownership, lifetime, persistence, invalidation, and concurrency belong
to a host or infrastructure layer.

### Query arbitrary persisted history

Rejected. This increment operates only on one exact caller-supplied validated chain.

### Return copied mutable checkpoint DTOs

Rejected. Exact immutable checkpoint and supersession authorities preserve lineage and
avoid divergent duplicate state.
