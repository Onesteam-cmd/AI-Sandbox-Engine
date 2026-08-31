# ADR-0048: Host Recovery Checkpoint Supersession and Completed-Cycle Summary Contracts

## Status

Accepted.

## Context

Commit 0047 closes one exact recovery cycle after terminal resumed-attempt settlement.
The completed cycle retains the original recovery checkpoint and the full immutable
checkpoint-to-terminal lineage. Core still needs an explicit way to state that a later
validated checkpoint supersedes the completed cycle's checkpoint and to expose a
compact completed-cycle projection without deleting, storing, archiving, compacting,
or executing anything.

Checkpoint capture, World Snapshot validation, recovery settlement, and cycle
completion already have separate bounded authority. Supersession must not recapture
state, mutate either checkpoint, infer a clock, or treat summary creation as persistence.

## Decision

Add immutable, externally identified, revisioned
`HostRuntimeRecoveryCheckpointSupersession<TRequest, TState, TCompletion>`
authority.

`HostRuntimeRecoveryCheckpointSupersessionFlow.Supersede`:

- consumes one exact successful recovery-cycle completion and one existing successor
  recovery checkpoint;
- validates optimistic cycle-completion and successor-checkpoint revisions;
- requires a new checkpoint identity;
- requires exact runtime, composition, queue, clock, and World identity lineage;
- requires successor checkpoint capture, checkpoint revision, World State version,
  logical simulation tick, and external supersession time not to regress;
- preserves both the prior and successor checkpoint authorities unchanged;
- creates no checkpoint, restore, retry, queue, lease, dispatch, attempt, or execution
  authority.

Add immutable, externally identified, revisioned
`HostRuntimeRecoveryCompletedCycleSummary<TRequest, TState, TCompletion>`
authority.

`HostRuntimeRecoveryCheckpointSupersessionFlow.Summarize` validates one exact
supersession revision and external monotonic summary time, then projects stable IDs,
terminal outcome, checkpoint/world versions, and lifecycle ticks. The summary retains
the supersession authority as evidence and does not serialize or persist it.

## Rejected alternatives

### Replace or delete the old checkpoint

Rejected because Core authorities are immutable evidence. Supersession is an explicit
relationship, not destructive mutation.

### Capture the successor checkpoint inside supersession

Rejected because checkpoint capture already has its own validated authority boundary
and externally supplied IDs, revisions, snapshots, and time.

### Store or archive completed-cycle summaries

Rejected because storage, retention, compaction, indexing, and archival policy are
outside generic Core contracts.

### Select retry or terminal disposition while summarizing

Rejected because retry, requeue, and dead-letter decisions remain independent of
recovery-cycle closure and checkpoint supersession.

## Consequences

- A completed cycle can be linked deterministically to a later checkpoint.
- Both checkpoint authorities and the complete terminal recovery lineage remain
  inspectable.
- Consumers can use a compact immutable summary without traversing every recovery
  contract for common identifiers and ticks.
- Core still does not own storage, deletion, archival, compaction, diagnostics,
  transport, scheduling, supervision, waiting, automatic restart, or execution.
