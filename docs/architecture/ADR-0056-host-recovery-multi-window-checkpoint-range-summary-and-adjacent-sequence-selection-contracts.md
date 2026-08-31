# ADR-0056: Host Recovery Multi-Window Checkpoint-Range Summary and Adjacent-Sequence Selection Contracts

## Status

Accepted.

## Context

Commit 0055 introduced immutable validation for one ordered bounded sequence of exact continuous-window pair summaries and a bounded inclusive query crossing one or more validated boundaries. Core still lacked a compact immutable summary of one exact multi-window range and explicit advisory authority selecting exact pair-summary sequences immediately before or after the pair interval intersected by that range.

## Decision

Core owns an externally identified revisioned multi-window checkpoint-range summary projection. Projection accepts one exact multi-window query, validates optimistic revision and non-regressing external time, retains the unchanged query as evidence, and exposes exact checkpoint, pair, crossed-boundary, incoming-boundary, outgoing-boundary, sequence, projection, and chain facts.

Core also owns an externally identified revisioned adjacent-sequence selection. Selection accepts one exact summary and a positive requested pair count bounded to eight, validates optimistic summary revision and non-regressing external time, and selects exact immediately previous or next pair summaries from the unchanged source sequence. Selection retains exact internal boundary supersessions and the exact supersession connecting the selected sequence to the summarized range pair interval.

Projection and selection do not discover, reorder, load, store, index, archive, delete, retain, compact, paginate, diagnose, restart, supervise, schedule, wait, transport, project a new sequence, mutate history, or execute.

## Consequences

- Exact multi-window query evidence can be retained as a compact immutable summary.
- One to eight exact previous or next pair summaries can be selected advisory from the source sequence.
- Internal selected-sequence boundaries and the single adjacent connecting supersession remain explicit immutable evidence.
- Adjacent-sequence projection and multi-window checkpoint-range continuity validation remain future contracts.
