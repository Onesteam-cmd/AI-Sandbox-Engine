# ADR-0047: Host Recovery Resumed-Attempt Settlement and Recovery-Cycle Completion Contracts

## Status

Accepted.

## Context

Commit 0046 can reconstruct a recovery dispatch and acknowledge it as a new resumed
in-flight attempt while preserving the complete checkpoint and recovery lineage.
The Core already has authoritative bounded contracts for terminal attempt settlement,
request finalization, lease release, retry requeue, and dead-letter disposition.

Recovery still needs two explicit authorities:

1. evidence that the acknowledged resumed attempt reached one exact terminal
   settlement through the existing attempt-settlement rules; and
2. evidence that the recovery cycle itself was closed after that settlement.

These authorities must not receive transport, execute work, choose retry policy,
dead-letter work, supervise workers, persist history, or own time.

## Decision

Add immutable externally identified
`HostRuntimeRecoveryResumedAttemptSettlement<TRequest, TState, TCompletion>`
authority. The recovery settlement flow:

- validates the optimistic resumed-attempt acknowledgement revision;
- requires a non-regressing externally supplied settlement tick;
- delegates exact request, lease, attempt, completion, worker, clock, and time
  validation to the existing attempt-settlement flow;
- delegates terminal routing and lease release to
  `HostRuntimeAttemptSettlementFlow.Settle`;
- exposes the underlying `HostRuntimeAttemptSettlementStatus` when settlement is
  rejected; and
- advances only the recovery settlement revision after successful settlement.

Add immutable externally identified
`HostRuntimeRecoveryCycleCompletion<TRequest, TState, TCompletion>` authority.
Cycle completion:

- validates the optimistic recovery settlement revision;
- requires a non-regressing externally supplied completion tick;
- preserves the complete checkpoint-to-settlement lineage; and
- advances only the recovery-cycle completion revision.

Recovery-cycle completion means that recovery orchestration for the exact checkpoint
attempt has ended. It does not reinterpret the business outcome. Completed, rejected,
failed, and cancelled attempt outcomes can all close the recovery cycle. Any retry,
requeue, dead-letter, or later operational decision remains governed by its existing
independent contract.

## Consequences

The Core gains explicit immutable closure authority for a recovered attempt and its
recovery cycle. Prior checkpoint, continuation, plan, selection, re-admission, lease
reacquisition, reconstruction, acknowledgement, request, lease, dispatch, and attempt
authorities remain unchanged evidence.

The Core still does not restart processes, supervise workers, receive transport,
execute completions, choose retries, requeue, dead-letter, schedule, persist, wait, or
read a hidden clock.
