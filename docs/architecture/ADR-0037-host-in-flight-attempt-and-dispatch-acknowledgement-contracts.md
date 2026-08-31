# ADR-0037: Host In-Flight Attempt and Dispatch Acknowledgement Contracts

- Status: Accepted
- Date: 2026-08-04
- Commit: 0037

## Context

Commit 0036 created immutable dequeue and dispatch selection authority, but Core
did not yet represent the boundary at which the selected worker explicitly
acknowledges that dispatch. A later completion must be attributable to a stable
attempt without turning generic Core into a queue executor, transport, thread
pool, or wall-clock owner.

## Decision

Core introduces an externally identified immutable
`HostRuntimeInFlightAttempt<TRequest>` and a pure
`HostRuntimeDispatchAcknowledgementFlow.Acknowledge` transition decision.

The acknowledgement validates:

- optimistic current request and lease revisions;
- pending request and active lease state;
- selection, request, lease, worker, dispatch, and attempt-number identity;
- the externally supplied monotonic clock domain and tick;
- lease acquisition and expiry boundaries.

A successful acknowledgement returns immutable in-flight attempt authority that
preserves the existing selection together with the current request and lease
authority, dispatch, routing, correlation, and priority lineage. Rejected acknowledgements are explicit and return no
attempt authority.

## Boundaries

This increment does not:

- execute the request;
- send or receive transport messages;
- own a concrete queue or worker;
- allocate IDs;
- read wall-clock time;
- schedule retries;
- create threads, timers, tasks, or background work;
- mutate authoritative world state.

## Consequences

Future settlement, cancellation, retry, and completion-routing increments can
key decisions to a stable acknowledged attempt. Host and game adapters remain
responsible for I/O, execution, ID allocation, and monotonic clock ownership.
