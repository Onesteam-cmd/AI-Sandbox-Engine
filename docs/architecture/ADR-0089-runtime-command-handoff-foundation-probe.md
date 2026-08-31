# ADR-0089: Runtime Command Handoff Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

Commit 0088 added a direct executable Command FoundationProbe. Its downstream
practical-value review selected `RuntimeOrchestrator` as a high-value production
consumer of the Command execution boundary.

A historical audit was required because commits 0083-0085 already introduced
HostRuntime foundation probes. That audit found no coverage of
`RuntimeOrchestrator`, `ExecuteCommand`, or `RuntimeCommandResult`; therefore
the Runtime command handoff is distinct rather than duplicate recovery work.

Creating another probe file would increase probe hierarchy without improving
the executable chain.

## Decision

Extend the existing `CommandProbe` with one adjacent Runtime command-handoff
scenario while preserving the direct 0088 scenario unchanged.

The Runtime scenario must:

1. build one `RuntimeOrchestrator` through `RuntimeOrchestratorBuilder`;
2. register one exact command handler;
3. execute one exact `CommandEnvelope` through `ExecuteCommand`;
4. require `RuntimeInvocationStatus.Completed`;
5. require `WasInvoked` and `WasCommitted`;
6. retain one Applied `CommandExecutionResult`;
7. evaluate the Runtime command handler exactly once;
8. advance authoritative world version exactly once while preserving simulation
   tick;
9. retain one valid command `RuntimeCommitFact`;
10. prove the Runtime does not dispatch that commit fact automatically.

No second probe file is added. No Core contract is added.

## Consequences

Foundation validation now spans the real Command-to-Runtime handoff without
discarding the direct CommandProcessor boundary and without increasing probe or
recovery hierarchy.

The next increment must value-gate distinct downstream consumers of this Runtime
handoff. Mechanical Runtime nesting is not permitted by this ADR.
