# ADR-0087: Conversation-to-Dialogue Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

Commit 0086 introduced an executable Conversation address-resolution probe and
explicitly required a value review before adding further FoundationProbe
coverage. That review identified DialogueOrchestrationProcessor as a
production-consumed executable boundary with direct Conversation authority,
existing test evidence, and no FoundationProbe coverage.

A separate Dialogue-only probe would duplicate world and conversation setup and
would weaken the integration evidence.

## Decision

Extend the existing ConversationProbe so Dialogue consumes the exact semantic
outputs of its preceding Conversation stage:

1. reuse the same ConversationState;
2. reuse the exact resolved AddressAudience;
3. construct one DialogueOrchestrationRequestEnvelope;
4. invoke one fixed pure orchestrator exactly once;
5. require DialogueOrchestrationStatus.Continued;
6. retain one stable Continue decision with the exact `invoke-model` directive;
7. do not execute the returned directive;
8. prove authoritative world state remains unchanged.

The returned directive is treated as semantic output only. Execution belongs to
a later bounded consumer increment if a separate value gate justifies it.

No Core contract is added.

## Consequences

Foundation validation now crosses a real Conversation-to-Dialogue production
boundary while preserving a single scenario chain rather than multiplying
isolated probes.

The next increment must review downstream Dialogue consumers for practical
value. Mechanical probe or contract expansion is not permitted by this ADR.
