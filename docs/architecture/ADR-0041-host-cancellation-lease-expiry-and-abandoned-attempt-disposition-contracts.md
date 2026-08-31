# ADR-0041: Host Cancellation, Lease-Expiry, and Abandoned-Attempt Disposition Contracts

- Status: Accepted
- Date: 2026-08-04
- Commit: 0041

## Context

Commit 0040 established terminal dead-letter authority for settled attempts whose
retry decision is exhausted. Core still lacked an explicit outcome for an
acknowledged attempt that ends without a completion because cancellation was
recorded or external monotonic time proved that its active lease expired.

## Decision

Core introduces an externally identified immutable
`HostRuntimeAbandonedAttemptDisposition<TRequest>` and one pure
`HostRuntimeAbandonedAttemptDispositionFlow.Dispose` decision.

Cancellation disposition requires current `CancellationRequested` request
authority before the exclusive lease-expiry boundary. It finalizes the request
as `Cancelled` and releases the exact attempt lease.

Lease-expiry disposition requires the external observed tick to reach the
exclusive expiry boundary. It expires the exact attempt lease and finalizes a
pending request as `Failed`; an already cancellation-requested request remains
semantically cancelled.

Both branches validate optimistic request and lease revisions, exact
attempt/request/lease/worker lineage, monotonic clock identity, and
post-acknowledgement time.

## Boundaries

This increment does not:

- interrupt a worker or provider;
- poll or read a wall clock;
- schedule recovery or retry;
- enumerate or store active attempts;
- create a concrete abandoned-work queue;
- execute request payloads;
- mutate external stored authority.

## Consequences

An external Host can now record why an acknowledged attempt ended without a
completion and receive terminal request plus released-or-expired lease
authority. Future active-work snapshot and reconciliation contracts can consume
this authority without inventing cancellation or expiry semantics.
