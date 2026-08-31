# ADR-0084: Host Runtime Retry, Requeue, and Dead-Letter Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

Commit 0083 added executable FoundationProbe coverage for the successful Host
Runtime queue-to-settlement lifecycle. Existing Core authorities also define
bounded retry decisions, immutable retry re-admission, and terminal
dead-letter disposition, but those cross-contract paths were represented only
by tests.

## Decision

Extend the FoundationProbe with two scenarios over one exact failed terminal
settlement:

1. evaluate an allowed retry under a bounded policy and apply it as immutable
   queue re-admission at the advisory retry tick;
2. evaluate an attempt-limit denial and convert it into immutable dead-letter
   disposition authority.

The probe must expose explicit statuses for the retry decision, requeue,
reopened request, resulting queue, exhausted decision, disposition, and
disposition kind.

Repository validation requires use of the existing retry-decision,
retry-requeue, and dead-letter flow authorities and execution of the new
probe path.

No new Core contract is introduced.

## Consequences

The foundation now has executable evidence for both continuing and terminal
failure handling. The external Host remains responsible for queue storage,
retry scheduling, waiting, transport, dead-letter storage, worker execution,
persistence, supervision, and clock ownership.
