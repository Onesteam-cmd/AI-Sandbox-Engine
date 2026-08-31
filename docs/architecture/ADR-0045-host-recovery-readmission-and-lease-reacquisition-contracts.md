# ADR-0045: Host Recovery Re-Admission and Lease Reacquisition Contracts

## Status

Accepted.

## Context

Commit 0044 can create an immutable recovery resumption plan and select one
pending checkpoint attempt. That selection is advisory: it deliberately does
not mutate a queue, acquire ownership, create a dispatch, or execute work.

The next bounded Core boundary must connect one exact resumed-work selection to
the existing queue-admission and worker-lease contracts without inventing a
parallel recovery queue or lease model.

## Decision

Core provides two pure immutable recovery authorities:

1. `HostRuntimeRecoveryReadmission<TRequest, TState>` records one successful
   re-admission of the exact selected pending request through
   `HostRuntimeQueueAdmissionFlow.Decide`.
2. `HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>` records one new
   active lease created through `HostRuntimeWorkLeaseFlow.Acquire` for that
   exact re-admission.

Re-admission validates:

- the optimistic resumed-work selection revision;
- non-regressing external monotonic re-admission time;
- exact prior, checkpoint, and current queue identity;
- use of a new admission ID rather than the checkpoint admission ID;
- the existing bounded queue revision and capacity rules.

Lease reacquisition validates:

- the optimistic recovery re-admission revision;
- non-regressing external monotonic reacquisition time;
- the recovery continuation clock domain;
- use of a new lease ID rather than the checkpoint lease ID;
- the existing bounded lease duration and identifier rules.

The prior request, admission, lease, worker, dispatch, and attempt remain
unchanged evidence. The new admission and lease are explicit new authorities.

## Consequences

Recovery can now prove that one selected checkpoint request was admitted into
the current bounded queue and received new worker ownership.

Core still does not:

- restart or supervise a process;
- discover workers or queues;
- dequeue or dispatch the re-admitted request;
- create a new dispatch or in-flight attempt;
- schedule, transport, persist, wait, or execute payloads;
- read wall-clock time or generate identifiers.

Those responsibilities remain external Host concerns or later bounded
contracts.
