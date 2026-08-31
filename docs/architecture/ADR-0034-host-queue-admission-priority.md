# ADR-0034: Host Queue Admission and Priority Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0034 Host Queue Admission and Priority Contracts`

## Context

Core now represents request, dispatch, retry, and completion authority. External
Hosts also need deterministic bounded admission and priority policy without
placing an actual queue, scheduler, worker pool, or synchronization primitive in
generic Core.

## Decision

Commit 0034 adds stable queue and admission IDs, immutable bounded queue
snapshots, explicit optimistic admission outcomes, four bounded priority classes,
and deterministic ordering by higher class then lower external sequence.

## Invariants

1. Queue and admission IDs are externally assigned.
2. Queue snapshots contain counts and revision, not stored requests.
3. Only pending requests are admission-eligible.
4. Stale queue revisions return explicit unchanged results.
5. Full capacity returns an explicit unchanged result.
6. Successful admission increments count and revision immutably.
7. Higher priority class precedes lower class.
8. Equal classes use lower externally assigned sequence first.
9. Core does not store, dequeue, schedule, synchronize, wait, or execute work.

## Deferred

Concrete queue collections, persistence, fairness windows, starvation policy,
worker selection, synchronization, dequeue, and dispatch execution remain Host
adapter responsibilities.
