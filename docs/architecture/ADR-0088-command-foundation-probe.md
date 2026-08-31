# ADR-0088: Command Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

The post-0087 value gate found no distinct production consumer downstream of
Dialogue, so further Dialogue probe expansion would have been mechanical.

A cross-domain practical-value ranking selected Commands. The formal ranking
surface included `ICommandHandler`, but that interface is a pure decision
boundary and does not own authoritative state mutation. The practical
executable boundary is `CommandProcessor.Execute`, which evaluates a handler
and applies an accepted immutable next state through `WorldStateManager`.

Appending this scenario to the existing Conversation-to-Dialogue probe would
create an artificial semantic chain.

## Decision

Add a separate bounded `CommandProbe` that:

1. creates one current immutable world snapshot;
2. registers one exact pure command handler;
3. constructs one exact `CommandEnvelope`;
4. executes `CommandProcessor` exactly once;
5. evaluates the handler exactly once;
6. requires `CommandExecutionStatus.Applied`;
7. requires the exact state transition `4 -> 7`;
8. requires one authoritative world-version advance `0 -> 1`;
9. requires simulation tick preservation.

The probe adds no queue, retry, timer, I/O, provider, or recovery behavior.

No Core contract is added.

## Consequences

Foundation validation now exercises a real command-to-authoritative-world-state
transition independently of the Conversation-to-Dialogue scenario.

The next increment must value-gate distinct production consumers of the Command
result or command execution boundary. Mechanical Command hierarchy expansion is
not permitted by this ADR.
