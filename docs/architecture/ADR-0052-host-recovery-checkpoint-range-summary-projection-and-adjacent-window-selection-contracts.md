# ADR-0052: Host Recovery Checkpoint-Range Summary Projection and Adjacent-Window Selection Contracts

## Status

Accepted.

## Context

Commit 0051 introduced exact bounded lineage-window projections and inclusive checkpoint-range queries. Core still lacked a compact immutable authority describing one resolved range and an advisory contract for selecting the exact previous or next bounded checkpoint interval adjacent to that range.

## Decision

Core owns an externally identified revisioned checkpoint-range summary projection that retains the unchanged range query as evidence and exposes compact checkpoint identities, source-window and source-chain indexes, counts, exact boundary authorities, and root/latest facts.

Core also owns an externally identified revisioned adjacent-window selection. Separate previous and next operations accept a positive requested checkpoint count bounded to 64, validate optimistic summary revision and non-regressing external time, and return exact source-chain indexes, first and last checkpoint authorities, and immediate incoming and outgoing supersession authorities.

Selection is advisory. It does not project a new lineage window, discover history, reorder evidence, paginate, load, store, index, archive, delete, retain, compact, diagnose, restart, supervise, schedule, wait, transport, or execute.

## Consequences

- Range consumers can rely on compact immutable summary evidence without traversing the full range.
- External orchestration can request deterministic previous or next adjacent windows while preserving the 64-checkpoint bound.
- Root and latest boundaries are explicit outcomes rather than inferred null behavior.
- Actual adjacent-window projection and continuity validation remain future bounded contracts.
