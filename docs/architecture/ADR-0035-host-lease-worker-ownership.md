# ADR-0035: Host Lease and Worker Ownership Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0035 Host Lease and Worker Ownership Contracts`

## Decision

Core now represents bounded worker ownership of one admitted Host request through
an externally clocked immutable lease. Acquisition, renewal, release, and expiry
preserve request, queue, admission, priority, worker, and clock authority.

## Invariants

1. Lease and worker IDs are externally assigned.
2. Lease duration uses an external monotonic clock and exclusive expiry tick.
3. Renewal, release, and expiry use optimistic lease revision.
4. Only the exact owner may release an active lease.
5. Expiry requires the matching clock and an observed tick at or beyond expiry.
6. Core never starts workers, polls clocks, waits, schedules, or executes work.
