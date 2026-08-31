# ADR-0031: Host Request Correlation and Cancellation Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0031 Host Request Correlation and Cancellation Contracts`

## Context

The Host now has immutable composition, lifecycle, and health authority. Requests
crossing the Host boundary still need stable correlation, optional parentage,
optimistic revisions, explicit terminal outcomes, and cancellation intent without
placing execution or transport policy in generic Core.

## Decision

Commit 0031 adds exact typed request and cancellation-reason payloads, stable
request/operation/correlation IDs, immutable request envelopes, optional parent
request correlation, explicit state transitions, and advisory cancellation
records.

## Invariants

1. Request, runtime, operation, and correlation IDs are externally assigned.
2. Parent request ID is optional and cannot equal the request itself.
3. Request and cancellation-reason payloads are exact value types or sealed
   reference types.
4. New requests begin pending at revision zero.
5. Cancellation changes only the immutable record to `CancellationRequested`.
6. Cancellation does not invoke `CancellationTokenSource`, stop work, retry, or
   signal transport.
7. Only pending or cancellation-requested records may become terminal.
8. Terminal records cannot transition again.
9. Stale revisions return explicit unchanged results.

## Deferred

Concrete queues, transport messages, deadlines, timeout policy, active
`CancellationToken` ownership, retries, and process interruption remain Host
adapter responsibilities.
