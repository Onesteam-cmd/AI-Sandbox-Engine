# ADR-0040: Host Retry Exhaustion and Dead-Letter Disposition Contracts

- Status: Accepted
- Date: 2026-08-04
- Commit: 0040

## Context

Commit 0039 established immutable retry requeue authority for allowed retries.
Existing retry-decision contracts also return explicit terminal denial when the
attempt limit is reached or an external monotonic deadline prevents another
attempt, but Core did not yet produce one stable authority that records why the
failed or rejected settlement must leave the active retry lifecycle.

## Decision

Core introduces an externally identified immutable
`HostRuntimeDeadLetterDisposition<TRequest, TCompletion>` and a pure
`HostRuntimeDeadLetterDispositionFlow.Dispose` decision.

The flow accepts terminal settlement authority, its exact denied retry decision,
an externally assigned disposition ID, and external monotonic time. It validates:

- failed or rejected terminal settlement outcome;
- exact terminal request authority;
- completed attempt lineage;
- monotonic clock identity;
- disposition time at or after settlement;
- denial of another retry;
- a supported terminal denial status.

`AttemptLimitReached` maps to an attempt-limit disposition and
`DeadlineExceeded` maps to a deadline disposition. Stale, invalid-state, or
otherwise non-terminal retry denials remain explicit rejected results rather
than being silently dead-lettered.

The resulting authority preserves settlement, completion, request, attempt,
worker, dispatch, policy, deadline, and retry-reason lineage. It does not create
or mutate a concrete dead-letter queue.

## Boundaries

This increment does not:

- store or transport dead-letter records;
- schedule, wait for, or execute retries;
- reopen or mutate terminal request authority;
- allocate IDs;
- read wall-clock time;
- persist disposition state;
- create threads, timers, tasks, or background work;
- mutate authoritative world state.

## Consequences

Host adapters can persist, route, inspect, or alert on explicit dead-letter
authority while Core remains deterministic and provider-neutral. Cancellation,
lease-expiry, and abandoned-attempt cleanup remain separate later lifecycle
contracts.
