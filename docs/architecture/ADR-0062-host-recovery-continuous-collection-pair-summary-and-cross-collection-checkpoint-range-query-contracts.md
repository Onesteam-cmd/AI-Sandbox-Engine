# ADR-0062: Host Recovery Continuous Collection-Pair Summary and Cross-Collection Checkpoint-Range Query Contracts

## Status

Accepted.

## Context

Commit 0061 introduced immutable adjacent-collection projection and an explicit
multi-sequence checkpoint-range continuity-validation authority proving one exact
connecting supersession between a summarized range and its projected previous or
next collection. Core still lacked a compact authority describing the resulting
continuous collection pair and a bounded query able to resolve one inclusive
checkpoint range crossing that shared collection boundary.

## Decision

Core defines an immutable externally identified revisioned
`HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<TRequest, TState,
TCompletion>` over one exact
`HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation<TRequest,
TState, TCompletion>`.

The summary preserves the unchanged range summary, projected adjacent collection,
source collection validation, source chain projection, validated chain, direction,
summary and checkpoint intervals, one exact connecting supersession and its
endpoint checkpoints, aggregate summary, sequence, pair, window, checkpoint and
supersession counts, source-collection boundary facts, and external projection time.

Core also defines an immutable externally identified revisioned
`HostRuntimeRecoveryCrossCollectionCheckpointRangeQuery<TRequest, TState,
TCompletion>`. The query accepts caller-supplied start and end checkpoint
identities, resolves one inclusive range within the collection-pair summary,
requires the range to cross the exact shared collection boundary, limits the
materialized result to 64 checkpoints, and preserves exact checkpoint,
supersession, incoming, outgoing, connecting-boundary and source-index evidence.

Both operations validate caller-supplied optimistic revisions and non-regressing
external ticks. They do not discover, reorder, load, store, index, archive, delete,
retain, compact, paginate, diagnose, restart, schedule, supervise, wait, transport,
mutate history, or execute.

## Consequences

- Hosts can compactly describe one exact continuous previous/range or range/next
  collection pair without copying or mutating recovery history.
- Hosts can resolve a bounded exact checkpoint range crossing the one validated
  collection boundary.
- Stale revisions, regressed time, source mismatch, non-continuous summary and
  checkpoint intervals, supersession mismatch, endpoint mismatch, missing
  checkpoints, invalid order, non-crossing ranges, oversized ranges, and
  materialized-boundary mismatch remain explicit outcomes.
- Core remains advisory and deterministic. Persistence, discovery, traversal,
  pagination, scheduling, transport, diagnostics, and execution remain Host or
  infrastructure responsibilities.
