# ADR-0036: Host Dequeue and Dispatch Selection Contracts

- **Status:** Accepted
- **Date:** 2026-07-27
- **Commit:** `0036 Host Dequeue and Dispatch Selection Contracts`

## Context

Queue admission, deterministic priority, worker leases, and advisory
dispatch records already exist. Generic Core still needs one explicit
boundary that validates an externally selected active lease, decrements
abstract queue authority, and produces a dispatch record without owning
a concrete queue or scheduler.

## Decision

Commit 0036 adds immutable dequeue-and-dispatch-selection authority.
External Host adapters choose the candidate from their concrete storage.
Core validates queue revision, non-empty count, queue identity, active
lease state, matching monotonic clock, and unexpired ownership.

## Invariants

1. Selection IDs are externally assigned.
2. Successful selection decrements queued count exactly once.
3. Successful selection increments queue revision exactly once.
4. The selected lease must belong to the represented queue.
5. The selected lease must be active and not externally expired.
6. Request, priority, worker, lease, route, and endpoint authority remain
   immutable.
7. Core does not enumerate, store, lock, poll, schedule, or execute a
   concrete queue.
