# ADR-0044: Host Recovery Resumption Planning and Resumed-Work Selection Contracts

## Status

Accepted.

## Context

Commit 0043 introduced immutable recovery checkpoint and continuation authority.
A successful continuation proves that an exact checksum-protected World State
snapshot was restored against one checkpoint, but it deliberately does not
restart processes, reacquire worker ownership, schedule requests, or execute
payloads.

The checkpoint also preserves a bounded deterministic active-work snapshot.
That snapshot can contain both pending requests and requests for which
cancellation has already been recorded. Recovery needs an explicit,
provider-neutral authority describing which checkpoint work may be considered
for resumption and an explicit selection of one planned attempt. Absence,
process restart, or restored World State alone must not silently create new
leases, dispatches, attempts, or execution authority.

## Decision

Core defines:

- externally identified immutable recovery resumption plans;
- deterministic pending candidates in stable attempt-ID order;
- explicit cancellation-requested attempt IDs suppressed from resumption;
- optimistic continuation and plan revision checks;
- external monotonic planning and selection ticks;
- immutable advisory selection of one exact planned active-work item;
- explicit stale, regressed-time, empty-plan, and missing-attempt outcomes.

Planning consumes one validated
`HostRuntimeRecoveryContinuation<TRequest, TState>` and reuses the exact
bounded active-work authority stored in its checkpoint. Only requests in
`Pending` state become candidates. Requests in `CancellationRequested` state
are recorded as suppressed attempt IDs. Planning does not infer new request,
lease, dispatch, attempt, or queue authority.

Selection consumes one immutable plan, validates the optimistic plan revision
and non-regressing selection time, then locates one exact candidate by stable
attempt ID. The resulting selection preserves the original attempt, request,
lease, worker, and dispatch lineage as evidence only.

## Consequences

The Host can persist, transport, or operationalize plans outside Core and can
decide how selected work is re-admitted or reacquires ownership in later
contracts. Core remains deterministic, bounded, immutable, and free from
provider-specific runtime behavior.

Cancellation intent is not lost during recovery: cancellation-requested work
cannot become resumable merely because it was present in a checkpoint.

## Rejected alternatives

### Automatically restart or dispatch every checkpoint attempt

Rejected because Core does not own processes, schedulers, workers, transports,
or execution.

### Treat all active-work snapshot entries as resumable

Rejected because `CancellationRequested` is current request authority and must
not be ignored.

### Create a new lease or attempt during selection

Rejected because selection is advisory evidence. Ownership reacquisition and
new attempt materialization require separate explicit contracts.

### Infer completion or abandonment from omitted plan candidates

Rejected because suppression records cancellation intent only. Other terminal
outcomes still require their dedicated settlement or disposition authority.
