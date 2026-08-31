# ADR-0030: Host Lifecycle and Health Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0030 Host Lifecycle and Health Contracts`

## Context

Commit 0029 provides a deterministic immutable capability composition, but generic
Core still needs a provider-neutral record of external Host lifecycle authority
and health without starting services, running probes, scheduling retries, or
owning infrastructure.

## Decision

Commit 0030 adds immutable runtime-instance snapshots, explicit lifecycle states,
optimistic revisions, exact typed health details, stable health-probe IDs, pure
direct lifecycle transitions, and pure health observations.

Lifecycle contracts record external facts. They do not start, stop, restart,
dispose, monitor, or probe anything.

## Invariants

1. Runtime-instance and composition IDs are externally assigned and non-empty.
2. New snapshots begin in `Created` at revision zero with `Unknown` health.
3. Only declared direct lifecycle edges are accepted.
4. Revisions advance exactly once per successful transition or observation.
5. Stale revisions return explicit results without retry.
6. Health observations require an active lifecycle state.
7. Health details are exact value types or sealed reference types.
8. `Stopped` is terminal and clears active health authority.
9. No wall clock, timers, background work, transport, DI, or process control
   enters generic Core.

## Deferred

Concrete service startup and shutdown, asynchronous health probes, watchdogs,
restart policy, process ownership, telemetry transport, and Unreal presentation
remain external Host responsibilities.
