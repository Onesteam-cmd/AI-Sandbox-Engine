# ADR-0049: Host Recovery Supersession-Chain Validation and Latest-Checkpoint Selection Contracts

## Status

Accepted.

## Context

Commit 0048 links one completed recovery cycle to one successor checkpoint and retains
both checkpoint authorities as immutable evidence. Multiple successful supersessions can
therefore describe an externally supplied checkpoint lineage, but Core has no bounded
authority that validates an ordered lineage or selects its exact latest checkpoint.

The new contracts must not discover files, load history, reorder persisted records,
capture checkpoints, mutate source authorities, or infer time. Storage, archival,
retention, compaction, diagnostics, automatic restart, scheduling, and execution remain
outside Core.

## Decision

Add immutable, externally identified, revisioned
`HostRuntimeRecoverySupersessionChain<TRequest, TState, TCompletion>` authority.

`HostRuntimeRecoverySupersessionChainFlow.Validate`:

- accepts one externally ordered bounded list of immutable checkpoint-supersession
  authorities plus the caller-observed revision for every edge;
- rejects empty and oversized lists, revision-count mismatch, stale revisions, duplicate
  supersession or checkpoint identities, disconnected edges, cycles, and shared
  checkpoint IDs representing different authority;
- requires exact runtime, composition, queue, clock, and World lineage;
- requires checkpoint revision, capture tick, World State version, logical simulation
  tick, supersession tick, and external validation time not to regress;
- snapshots the ordered references without storage or history mutation;
- preserves every supplied supersession, checkpoint, and completed recovery cycle.

Add immutable, externally identified, revisioned
`HostRuntimeRecoveryLatestCheckpointSelection<TRequest, TState, TCompletion>`
authority.

`HostRuntimeRecoverySupersessionChainFlow.SelectLatest`:

- consumes one exact successful supersession-chain authority;
- validates the optimistic chain revision and non-regressing external selection time;
- selects the exact successor checkpoint of the final validated supersession edge;
- creates no checkpoint, continuation, queue, lease, dispatch, attempt, restore, retry,
  or execution authority.

## Consequences

Core can validate a bounded ordered recovery checkpoint lineage and expose its exact
latest checkpoint without storage, archival, deletion, compaction, scheduling,
supervision, waiting, restart, transport, diagnostics, or execution.

The caller remains responsible for obtaining and ordering supersession authorities,
assigning IDs, supplying revisions and monotonic ticks, and deciding whether or where
the validated chain or selection is retained.
