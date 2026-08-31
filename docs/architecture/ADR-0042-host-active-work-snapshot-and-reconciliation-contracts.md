# ADR-0042: Host Active-Work Snapshot and Reconciliation Contracts

- Status: Accepted
- Date: 2026-08-04
- Commit: 0042

## Context

Commit 0041 established explicit terminal authority for acknowledged attempts
that end without a completion because cancellation was recorded or an active
lease expired. Core still lacked one bounded immutable representation of work
that remains active at an externally observed monotonic tick, and it could not
deterministically compare consecutive active-work observations.

## Decision

Core introduces:

- `HostRuntimeActiveWorkItem<TRequest>` to group one acknowledged attempt with
  current request and lease authority;
- `HostRuntimeActiveWorkSnapshot<TRequest>` as an externally identified,
  revisioned, bounded, deterministically ordered active-work snapshot;
- `HostRuntimeActiveWorkReconciliation<TRequest>` as immutable added, retained,
  and removed attempt-ID authority between sequential snapshots;
- `HostRuntimeActiveWorkFlow.Capture` and
  `HostRuntimeActiveWorkFlow.Reconcile` as pure decisions.

Snapshot capture validates runtime, clock, attempt/request/lease/worker lineage,
non-regressing current authority revisions, active request and lease states, and
acknowledgement/expiry time boundaries. It permits an empty snapshot and
defensively copies at most 256 items ordered by stable attempt ID.

Reconciliation validates optimistic and sequential snapshot revisions, runtime
and clock identity, non-regressing observation time, stable retained-attempt
lineage, and non-regressing retained request and lease revisions. Removed
attempts are recorded only as absent; Core does not infer completion,
abandonment, dead-lettering, or any other terminal reason from absence alone.

## Boundaries

This increment does not:

- enumerate processes, workers, transports, or queues;
- poll or discover active work;
- persist or restore snapshots;
- execute, interrupt, cancel, retry, or reschedule requests;
- read wall-clock time;
- infer terminal outcomes from missing observations;
- create background services, locks, timers, or threads.

## Consequences

Host adapters can capture explicit active-work authority and compare sequential
observations without losing deterministic lineage. Later recovery and
continuation contracts can require these snapshots and reconciliation results
without making generic Core own discovery, persistence, or execution.
