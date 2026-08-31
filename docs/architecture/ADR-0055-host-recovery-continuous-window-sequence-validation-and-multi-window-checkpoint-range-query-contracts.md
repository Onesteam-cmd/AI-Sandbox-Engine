# ADR-0055: Host Recovery Continuous-Window Sequence Validation and Multi-Window Checkpoint-Range Query Contracts

## Status

Accepted.

## Context

Commit 0054 introduced immutable summaries for one exact continuous recovery-window pair and a bounded query crossing its shared boundary. Core still lacked explicit authority for validating an ordered bounded sequence of such pairs and resolving one exact checkpoint range across any validated boundaries retained by that sequence.

## Decision

Core owns an externally identified revisioned continuous-window sequence-validation authority. Validation accepts one caller-supplied ordered collection of one to eight exact pair summaries and matching optimistic revisions, validates non-regressing external time, exact shared source projection and chain identity, unique pair-summary identities, every internal pair boundary, exact non-overlapping pair order, and every supersession connecting consecutive pairs.

Core also owns an externally identified revisioned multi-window checkpoint-range query. Query accepts one exact sequence validation and exact inclusive start and end checkpoint identities, validates optimistic revision and non-regressing external time, requires the range to cross at least one validated window boundary, limits the result to 64 checkpoints, and returns exact checkpoint, supersession, crossed-boundary, incoming-boundary, and outgoing-boundary evidence.

Validation and query do not discover, reorder, load, store, index, archive, delete, retain, compact, paginate, diagnose, restart, supervise, schedule, wait, transport, mutate history, or execute.

## Consequences

- One to eight exact continuous-window pair summaries can be validated as a bounded immutable sequence.
- Exact internal and inter-pair supersession boundaries remain explicit immutable evidence.
- Inclusive checkpoint ranges can cross one or more validated window boundaries while remaining bounded to 64 checkpoints.
- Multi-window checkpoint-range summary projection and adjacent-sequence selection remain future contracts.
