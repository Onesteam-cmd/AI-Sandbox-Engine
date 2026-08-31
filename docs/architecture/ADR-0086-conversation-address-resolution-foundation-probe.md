# ADR-0086: Conversation Address Resolution Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

The Host Runtime FoundationProbe branch now covers its practical lifecycle
through successful settlement, retry/requeue, dead-letter disposition,
cancellation abandonment, and lease-expiry abandonment. A transitive call-chain
audit confirmed that the remaining direct method-count gaps are already
executed by those scenarios or have no distinct production-backed probe value.

The next-domain value gate ranked Conversation as the strongest practical
integration target. Within Conversation, marker contracts score highly by
reference count but are not useful executable entry points. The concrete
AddressResolutionProcessor is consumed by Dialogue and Social orchestration and
provides a bounded semantic path suitable for a FoundationProbe.

## Decision

Add one self-contained Conversation FoundationProbe scenario using only
existing Core authorities:

1. create a world-state manager whose authority is read but not mutated;
2. start a three-participant ConversationState at revision one;
3. create one exact participant audience and one fixed resolver decision;
4. execute AddressResolutionProcessor once with an exact immutable request;
5. require Resolved status, one resolver invocation, stable decision retention,
   and the exact audience instance;
6. prove the authoritative world ID, version, simulation tick, and state are
   unchanged.

Repository verification requires both the ConversationProbe source surface and
its invocation from the executable FoundationProbe.

No new Core contract is introduced.

## Consequences

Foundation validation now crosses from Host Runtime into a production-consumed
Conversation semantic path without mechanically extending the recovery or
contract hierarchy.

The next increment must review the existing Conversation-to-Dialogue consumer
chain for practical integration value before adding another probe or contract.
