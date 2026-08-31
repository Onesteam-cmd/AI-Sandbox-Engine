# ADR-0059: Host Recovery Continuous Multi-Sequence Collection Validation and Multi-Sequence Checkpoint-Range Query Contracts

## Status

Accepted.

## Context

Commit 0058 introduced immutable summaries over one exact continuous recovery multi-sequence and a bounded query crossing its shared sequence boundary. Core still lacked explicit authority for validating an ordered bounded collection of such summaries and resolving one exact checkpoint range across any validated sequence boundaries retained by that collection.

## Decision

Core owns an externally identified revisioned continuous multi-sequence collection-validation authority. Validation accepts one caller-supplied ordered collection of one to eight exact multi-sequence summaries and matching optimistic revisions, validates non-regressing external time, exact shared source projection and chain identity, unique summary identities, each summary's internal connecting boundary, exact non-overlapping pair and checkpoint order, and every supersession connecting consecutive summaries.

Core also owns an externally identified revisioned multi-sequence checkpoint-range query. Query accepts one exact collection validation and exact inclusive start and end checkpoint identities, validates optimistic revision and non-regressing external time, requires the range to cross at least one validated sequence boundary, limits the result to 64 checkpoints, and returns exact checkpoint, supersession, crossed-boundary, incoming-boundary, outgoing-boundary, and source-summary-index evidence.

Validation and query do not discover, reorder, load, store, index, archive, delete, retain, compact, paginate, diagnose, restart, supervise, schedule, wait, transport, mutate history, or execute.

## Consequences

- One to eight exact continuous multi-sequence summaries can be validated as a bounded immutable collection.
- Exact internal and inter-summary supersession boundaries remain explicit immutable evidence.
- Inclusive checkpoint ranges can cross one or more validated sequence boundaries while remaining bounded to 64 checkpoints.
- Multi-sequence checkpoint-range summary projection and adjacent-collection selection remain future contracts.
