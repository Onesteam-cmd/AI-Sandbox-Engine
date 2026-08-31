# ADR-0033: Host Dispatch and Completion Routing Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0033 Host Dispatch and Completion Routing Contracts`

## Context

Host request, cancellation, deadline, and retry authority now exists in Core.
External adapters still need stable dispatch identity and deterministic completion
matching without moving transport, endpoint I/O, provider calls, or process
execution into generic Core.

## Decision

Commit 0033 adds stable dispatch, route, and endpoint IDs; immutable advisory
dispatch envelopes; exact typed completion payloads; explicit completion kinds;
and pure completion routing that matches identity and finalizes request authority.

## Invariants

1. Dispatch, route, and endpoint IDs are externally assigned.
2. Only pending requests may create a dispatch record.
3. Dispatch attempt numbers are one-based and bounded.
4. Completion payloads are exact value types or sealed reference types.
5. Completion identity must exactly match dispatch and request identity.
6. Current request revision remains optimistic authority.
7. Pending and cancellation-requested records may accept terminal completion.
8. Routing maps completion kind to existing terminal request state.
9. Routing never sends, receives, waits, invokes a provider, or executes payloads.

## Deferred

Concrete queues, IPC, HTTP, provider SDKs, worker selection, endpoint discovery,
serialization, retries, and process ownership remain Host adapter concerns.
