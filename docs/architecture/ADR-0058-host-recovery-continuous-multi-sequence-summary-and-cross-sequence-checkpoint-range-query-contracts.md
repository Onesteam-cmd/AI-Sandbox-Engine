# ADR-0058: Host Recovery Continuous Multi-Sequence Summary Projection and Cross-Sequence Checkpoint-Range Query Contracts

## Status

Accepted.

## Context

Commit 0057 introduced exact adjacent-sequence projection and immutable continuity validation between one summarized multi-window checkpoint range and one projected previous or next sequence. Core still lacked one compact authority representing the resulting continuous multi-sequence interval and one bounded query authority for exact checkpoint ranges that cross its shared sequence boundary.

## Decision

Core owns an externally identified revisioned continuous multi-sequence summary projection authority. Projection accepts one exact multi-window checkpoint-range continuity validation, validates optimistic continuity revision and non-regressing external time, verifies exact summary origin, immediate pair-index and checkpoint-index continuity, one shared connecting supersession, and exact prior and successor checkpoint authorities. The summary preserves the unchanged range summary, adjacent-sequence projection, source sequence, source projection, chain, pair indexes, checkpoint indexes, counts, direction, and connecting boundary.

Core also owns an externally identified revisioned bounded cross-sequence checkpoint-range query authority. Query accepts one exact continuous multi-sequence summary, two checkpoint identities, optimistic summary revision, and external query time. It resolves one inclusive source-chain interval of at most 64 checkpoints, requires the interval to cross the exact connecting supersession, and preserves exact checkpoints, supersessions, incoming and outgoing authorities, source indexes, and the unchanged summary.

Projection and query do not discover, reorder, load, store, index, archive, delete, retain, compact, paginate, diagnose, restart, supervise, schedule, wait, transport, mutate history, or execute.

## Consequences

- One compact immutable authority now represents an exact continuous range-and-adjacent-sequence interval.
- Cross-sequence checkpoint-range queries prove that the selected interval crosses the validated sequence boundary.
- Pair, checkpoint, supersession, direction, and source-chain evidence remain exact immutable authorities.
- Continuous multi-sequence collection validation and broader bounded multi-sequence queries remain future contracts.
