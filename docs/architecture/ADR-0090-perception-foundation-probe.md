# ADR-0090: Perception Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

After commit 0089, the Runtime command-handoff downstream value gate found no
distinct production consumer and closed that branch. A new practical-value
ranking selected Perception.

The ranking's formal top target was `IPerceptionEvaluator`, but that interface is
a deliberately pure decision boundary. The executable target is
`PerceptionProcessor.Evaluate`, which version-gates a candidate stimulus, invokes
one evaluator, and may produce a `PerceptionObservation` without mutating
authoritative World State.

The existing FoundationProbe already has domain-specific HostRuntime,
Conversation/Dialogue, and Command/Runtime probes. Reusing those files for
Perception would mix unrelated semantics. No generic low-coupling probe exists.

## Decision

Allow one reviewed exception to the normal prohibition on automatic probe-file
growth and add exactly one `PerceptionProbe.cs`.

The probe must:

1. create one immutable authoritative world snapshot;
2. create one exact `PerceptionStimulusEnvelope`;
3. register one pure evaluator through `PerceptionProcessorBuilder`;
4. evaluate the stimulus through `PerceptionProcessor`;
5. require evaluator execution exactly once;
6. require an Observed result with one `PerceptionObservation`;
7. validate observation stimulus, channel, observer, world, version, simulation
   tick, confidence, and signal;
8. retain the exact authoritative source snapshot;
9. prove authoritative World State reference, state, version, and simulation
   tick remain unchanged.

Perception is not converted into knowledge, memory, belief, command, event, or
persistence by this probe.

## Consequences

The executable FoundationProbe now covers a real Perception decision boundary
without changing Core contracts or introducing recovery behavior.

The next increment must value-gate distinct production consumers of Perception.
Mechanical nesting of Perception contracts or probe files is not permitted.
