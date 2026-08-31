# ADR-0038: Host Attempt Settlement and Terminal Outcome Contracts

- Status: Accepted
- Date: 2026-08-04
- Commit: 0038

## Context

Commit 0037 established immutable in-flight attempt authority after explicit
worker acknowledgement. Existing completion routing could terminalize request
authority, and existing lease transitions could release worker ownership, but
Core did not yet provide one bounded decision that proved both outcomes belonged
to the same acknowledged attempt.

## Decision

Core introduces an externally identified immutable
`HostRuntimeAttemptSettlement<TRequest, TCompletion>` and a pure
`HostRuntimeAttemptSettlementFlow.Settle` decision.

Settlement validates:

- optimistic current request and lease revisions;
- request and lease ownership by the acknowledged attempt;
- routable request and active lease states;
- exact worker and monotonic clock identity;
- acknowledgement and lease-expiry time boundaries;
- completion identity through the existing completion router.

A successful decision uses the existing request-finalization and lease-release
contracts, then returns immutable settlement authority containing the unchanged
attempt and completion, terminal request authority, released lease authority,
and the externally supplied settlement tick.

## Boundaries

This increment does not:

- execute request or completion payloads;
- receive or send transport messages;
- persist or store attempts;
- schedule retries or requeue work;
- allocate IDs;
- read wall-clock time;
- create tasks, threads, timers, or background work;
- mutate authoritative world state.

## Consequences

Every terminal Host outcome can now be attributed to one stable acknowledged
attempt while proving that request authority became terminal and worker
ownership was released. Retry and requeue policy remains a later bounded
decision over failed or rejected settlement outcomes.
