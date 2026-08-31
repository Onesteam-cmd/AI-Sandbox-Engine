# ADR-0046: Host Recovery Dispatch Reconstruction and Resumed-Attempt Acknowledgement Contracts

- Status: Accepted
- Date: 2026-08-04
- Commit: 0046

## Context

Commit 0045 established recovery queue re-admission and a new active lease for one
selected checkpoint attempt. Core still lacked explicit authority to reconstruct a
new advisory dispatch from that lease and to acknowledge the reconstructed dispatch
as a new resumed in-flight attempt.

The prior checkpoint dispatch and attempt must remain immutable evidence. Recovery
must not silently reuse their identities, bypass existing dispatch-selection or
acknowledgement validation, or imply transport and execution.

## Decision

Core introduces:

- `HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>` as immutable
  authority over one new dispatch selection derived from exact lease-reacquisition
  authority;
- `HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>` as immutable
  authority over one new acknowledged resumed attempt;
- `HostRuntimeRecoveryDispatchFlow.Reconstruct` to validate optimistic
  lease-reacquisition and queue revisions, external monotonic time, queue lineage,
  new selection and dispatch identities, and the exact next attempt number before
  delegating to existing dispatch-selection contracts;
- `HostRuntimeRecoveryDispatchFlow.Acknowledge` to validate optimistic
  reconstruction, external monotonic time, a new attempt identity, and existing
  request, lease, worker, dispatch, request-ID, attempt-number, clock, and expiry
  acknowledgement rules;
- explicit wrapper results retaining the underlying dispatch-selection or
  dispatch-acknowledgement status when those existing contracts reject the action.

Prior request, admission, lease, worker, dispatch, selection, and attempt authority
remains unchanged evidence. Reconstruction and acknowledgement advance only their
own immutable recovery authority revisions.

## Consequences

- Recovery dispatch and resumed-attempt lineage is explicit and auditable.
- Existing dequeue, dispatch, and acknowledgement validation remains authoritative.
- Prior selection, dispatch, and attempt identities cannot be reused.
- Core creates advisory authority only; it does not transport a dispatch, contact a
  worker, schedule work, wait, supervise processes, persist, or execute a payload.
- External code owns IDs, time, worker availability, transport, and actual execution.
