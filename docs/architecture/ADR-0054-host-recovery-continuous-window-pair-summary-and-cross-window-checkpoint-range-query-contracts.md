# ADR-0054: Host Recovery Continuous-Window Pair Summary and Cross-Window Checkpoint-Range Query Contracts

## Status

Accepted.

## Context

Commit 0053 introduced immutable adjacent-window projection and an explicit continuity-validation authority proving one exact connecting supersession between a summarized range and its projected previous or next window. Core still lacked a compact authority describing the resulting continuous pair and a bounded query able to resolve one inclusive checkpoint range crossing that shared boundary.

## Decision

Core owns an externally identified revisioned continuous-window pair summary projection. Projection accepts one exact immutable continuity validation, validates optimistic revision and non-regressing external time, preserves the unchanged source range, adjacent window, validated chain, connecting supersession, boundary checkpoints, pair indexes, counts, and root/latest facts.

Core also owns an externally identified revisioned cross-window checkpoint-range query. Query accepts one exact pair summary and exact inclusive start and end checkpoint identities, validates optimistic revision and non-regressing external time, requires the range to cross the shared boundary, limits the result to 64 checkpoints, and returns exact checkpoint, supersession, incoming-boundary, outgoing-boundary, and connecting-supersession evidence.

Projection and query do not discover, reorder, load, store, index, archive, delete, retain, compact, paginate, diagnose, restart, supervise, schedule, wait, transport, mutate history, or execute.

## Consequences

- One validated previous/range or range/next pair has a compact immutable summary authority.
- Exact bounded ranges can cross one validated window boundary without reconstructing or mutating lineage history.
- Query results preserve the same connecting supersession and exact chain authorities.
- Continuous-window sequence validation and bounded multi-window queries remain future contracts.
