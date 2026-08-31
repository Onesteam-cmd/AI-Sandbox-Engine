# ADR-0092: Prompt Budget Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

After commit 0091, the Social downstream-consumer practical-value gate found
zero distinct production consumers and closed the Social branch.

A fresh practical-value ranking selected Prompting. Exact executable-chain
discovery compared `PromptBudgetManager` and `PromptCompositionProcessor`.
The composition processor did not satisfy the required authority-change
criterion, while the budget manager is a stateless deterministic production
boundary with reference-test evidence and a real composition consumer.

No existing Prompting FoundationProbe file exists. `Program.cs` is already an
orchestration surface rather than a domain scenario host, and no generic
low-coupling probe can absorb Prompting without semantic mixing.

## Decision

Allow one reviewed exception to the normal prohibition on automatic probe-file
growth and add exactly one `PromptingProbe.cs`.

The probe must:

1. define one exact local `IPromptContent` payload type;
2. create one required candidate and two optional candidates with distinct
   stable IDs, priorities, costs, owner scope, and world scope;
3. deliberately submit the candidates in non-priority order;
4. invoke `PromptBudgetManager.Allocate` exactly once with a 10-unit budget;
5. require `PromptBudgetStatus.Selected`;
6. require exact accounting of 4 required, 10 used, and 0 remaining units;
7. require selection of the required candidate and the highest-priority fitting
   optional candidate;
8. require the lower-priority non-fitting optional candidate to be skipped;
9. require deterministic selected ordering independent of input order;
10. preserve exact selected-object identity;
11. preserve input candidate identity, payload values, owner scope, and world
    scope.

The probe must not introduce World State mutation, Conversation, Dialogue,
Command, Runtime, Perception, Social, PromptComposition execution, model
provider I/O, network transport, persistence, retry scheduling, timers,
background workers, or wall-clock behavior.

## Consequences

FoundationProbe now covers a practical deterministic Prompting budget boundary
without changing Core contracts or authority semantics.

The next increment must value-gate distinct production consumers of Prompting.
Further mechanical Prompting contract nesting or automatic probe-file growth is
not permitted.
