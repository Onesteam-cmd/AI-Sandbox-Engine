# ADR-0039: Host Retry Requeue and Re-Admission Contracts

- Status: Accepted
- Date: 2026-08-04
- Commit: 0039

## Context

Commit 0038 established immutable terminal settlement authority for one
acknowledged attempt. Existing retry decisions could advise whether another
attempt was permitted, and queue-admission contracts could admit a pending
request, but Core did not yet provide one bounded authority that reopened an
eligible failed or rejected request and proved its re-admission belonged to the
same settlement and retry decision.

## Decision

Core introduces an externally identified immutable
`HostRuntimeRetryRequeue<TRequest, TCompletion>` and a pure
`HostRuntimeRetryRequeueFlow.Requeue` decision.

The flow accepts terminal settlement authority, an advisory retry decision,
current queue authority, externally assigned IDs, deterministic priority, and
external monotonic time. It validates:

- failed or rejected settlement outcome;
- an allowed retry decision for the exact terminal request authority;
- completed and next-attempt lineage;
- monotonic clock identity;
- retry-decision, settlement, and retry-at time boundaries;
- optimistic queue revision and bounded queue capacity.

Only after all retry checks pass does the flow create a new pending request
revision with the same stable request, runtime, operation, correlation, parent,
and payload lineage. Existing queue-admission contracts then atomically produce
the resulting queue snapshot and admission authority. Rejected results expose
the unchanged terminal request and queue authority.

## Boundaries

This increment does not:

- schedule or wait for the retry tick;
- execute the next attempt;
- own concrete queue storage;
- allocate IDs or priority sequences;
- read wall-clock time;
- persist retry state;
- create threads, timers, tasks, or background work;
- mutate authoritative world state.

## Consequences

Host adapters may persist and schedule the returned authority while Core retains
pure deterministic validation of retry lineage and re-admission. Exhausted or
otherwise denied retries remain explicit and can be routed by later
dead-letter/disposition contracts.
