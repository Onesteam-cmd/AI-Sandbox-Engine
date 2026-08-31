# ADR-0051: Host Recovery Lineage-Window Projection and Bounded Checkpoint-Range Query Contracts

## Status

Accepted.

## Context

Commit 0050 introduced one exact immutable chain-summary projection and exact
single-checkpoint lineage queries. Callers also require bounded contiguous views over
that validated lineage and exact inclusive subrange resolution without introducing
history discovery, storage, indexing, pagination state, or execution behavior.

## Decision

Core defines an immutable externally identified revisioned lineage-window projection
over one exact chain-summary projection.

A window is selected by a zero-based source-chain checkpoint index and positive
checkpoint count. One window represents at most 64 checkpoints, retains exact
checkpoint and internal supersession authorities, and exposes immediate incoming and
outgoing boundary supersessions when they exist.

Core also defines an immutable externally identified revisioned checkpoint-range query
over one exact lineage window. A query resolves exact inclusive start and end
checkpoint identities, returns their window and chain indexes, exact checkpoints,
internal supersessions, and immediate range-boundary supersessions.

Both operations validate optimistic source revisions and externally supplied monotonic
ticks. Invalid source indexes, oversized windows, missing range boundaries, and
reversed ranges are explicit outcomes.

## Consequences

- Source projections, validated chains, checkpoints, supersessions, and completed-cycle
  authorities remain unchanged evidence.
- Range queries cannot escape the bounded source window.
- Core does not discover, reorder, load, store, index, archive, delete, retain,
  compact, paginate, diagnose, restart, supervise, schedule, wait, transport, or
  execute.
- Adjacent-window navigation and checkpoint-range summary projection remain future
  bounded contracts.
