# ADR-0032: Host Deadline and Retry Decision Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0032 Host Deadline and Retry Decision Contracts`

## Context

Commit 0031 records immutable Host request authority and cancellation intent.
External Host adapters still need portable deadline and retry decisions without
placing clocks, timers, waits, schedulers, or active retries in generic Core.

## Decision

Commit 0032 adds externally owned monotonic clock IDs, immutable exclusive
deadlines, bounded retry policies, exact typed retry reasons, and pure advisory
retry decisions for failed or rejected request records.

## Invariants

1. Core never reads a wall clock.
2. Clock, policy, request, and reason authority is supplied externally.
3. Deadline ticks are non-negative and exclusive.
4. Retry policies allow 1–32 total attempts and a bounded advisory delay.
5. Only `Failed` and `Rejected` requests are retry-eligible.
6. Stale request revisions return an explicit unchanged decision.
7. A retry at or beyond the deadline is denied.
8. Reaching the inclusive attempt limit is explicit.
9. Decisions do not wait, schedule, enqueue, mutate, or execute anything.

## Deferred

Concrete timers, queues, jitter, backoff algorithms, provider-specific error
classification, cancellation tokens, and retry execution remain Host adapter
responsibilities.
