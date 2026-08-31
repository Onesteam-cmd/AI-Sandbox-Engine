# ADR-0085: Host Runtime Cancellation, Lease-Expiry, and Abandonment Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

Commits 0083 and 0084 added executable FoundationProbe coverage for successful
settlement and retry or terminal failure handling. The remaining bounded Host
Runtime lifecycle gap was acknowledged work that becomes abandoned after
recorded cancellation intent or externally proven lease expiry.

The existing abandoned-attempt authority is immutable and composes request and
lease transition contracts. Core does not observe clocks, interrupt work, or
monitor leases.

## Decision

Extend the FoundationProbe with two scenarios over one acknowledged attempt:

1. request cancellation before lease expiry, then produce abandonment
   authority that cancels the request and releases the active lease;
2. externally observe the lease-expiry boundary, then produce abandonment
   authority that fails the pending request and expires the lease.

Expose explicit disposition status, disposition kind, request state, and lease
state for both scenarios. Repository validation requires the cancellation and
abandoned-attempt flow calls plus executable probe invocation.

No new Core contract is introduced.

## Consequences

The FoundationProbe now covers successful settlement, retry and requeue,
dead-letter disposition, cancellation abandonment, and lease-expiry
abandonment. The next increment must review coverage and rank the next domain
by practical value rather than automatically adding another Host Runtime
scenario.

The external Host remains responsible for interruption, cancellation
signalling, clock observation, lease monitoring, queue storage, worker
execution, persistence, supervision, and wall-clock ownership.
