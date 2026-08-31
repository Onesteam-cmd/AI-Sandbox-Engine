# ADR-0060: Host Recovery Multi-Sequence Checkpoint-Range Summary Projection and Adjacent-Collection Selection Contracts

## Status

Accepted.

## Context

Commit 0059 introduced exact bounded validation for ordered collections of continuous multi-sequence summaries and exact inclusive checkpoint-range queries crossing validated sequence boundaries. The next bounded Core increment needs compact immutable summary authority over one exact query and explicit advisory selection of immediately adjacent summaries without discovering, reordering, loading, storing, indexing, or executing anything.

## Decision

Core exposes an externally identified revisioned multi-sequence checkpoint-range summary projection that preserves the unchanged query, collection validation, source projection, chain, exact checkpoint interval, intersected source-summary interval, crossed boundaries, incoming and outgoing supersessions, counts, external monotonic tick, and optimistic revision.

Core also exposes externally identified revisioned previous/next adjacent-collection selection. A selection accepts a positive caller-supplied count bounded to eight summaries, selects only summaries immediately before or after the summarized source interval, preserves source order, exact internal boundary supersessions, the one supersession connecting the selection to the source range, incoming and outgoing supersessions, indexes, counts, external monotonic tick, and optimistic revision.

Failure remains explicit for stale revisions, regressed ticks, oversized counts, absent or insufficient adjacent summaries, and boundary-evidence mismatch. Failed results preserve the exact source authority and materialize no selection.

## Consequences

The Host can reason over compact multi-sequence range evidence and request exact adjacent collection slices without Core performing discovery, reordering, storage, indexing, archival, deletion, retention, compaction, pagination, diagnostics, restart, scheduling, supervision, waiting, transport, history mutation, or execution.
