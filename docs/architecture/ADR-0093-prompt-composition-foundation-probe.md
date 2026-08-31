# ADR-0093: Prompt Composition Foundation Probe

- Status: Accepted
- Date: 2026-08-07

## Context

Commit 0092 established executable deterministic prompt budgeting in the
existing `PromptingProbe`. The required downstream-consumer review identified
`PromptCompositionProcessor` as the concrete production consumer of that budget
result and therefore as the smallest next executable boundary that reduces
integration uncertainty.

Transient CORE-0093 validation V3 compiled and ran successfully against exact
HEAD `a9afeaad956df1499d88ce14a624cb78cd8ca85e`. It established the exact API
shape: `PromptWorldState` must be a reference type; composition uses
`context.Snapshot`, `context.BudgetResult.SelectedCandidates`, and
`result.Decision`; World State version remains strongly typed and simulation
tick remains `ulong`.

The existing `PromptingProbe.cs` can absorb this adjacent composition scenario
without semantic mixing. Creating another Prompting probe file or new Core
contract would add structure without product value.

## Decision

Extend the existing `PromptingProbe` with one separate `RunComposition()`
scenario.

The scenario must:

1. use one reference-type immutable `PromptWorldState`;
2. create one exact owner/world/version/tick-scoped prompt request;
3. reuse the bounded 10-unit required/optional candidate shape;
4. create one fixed pure composer and one exact prompt document;
5. create one `PromptCompositionProcessor`;
6. invoke `PromptCompositionProcessor.Compose` exactly once;
7. require `PromptCompositionStatus.Composed`;
8. require exactly one composer invocation;
9. require the composer to observe the exact authoritative snapshot, request,
   composer identity, and deterministic selected candidates;
10. require exact 4 required, 10 used, and 0 remaining budget units;
11. preserve request and document identity;
12. validate document ID, composer ID, owner, world, cost, and payload;
13. prove authoritative World State reference/value/version/simulation tick are
    unchanged after composition.

No provider invocation, structured model decoding, gameplay action, persistence,
queue/executor, scheduling, timer, background worker, recovery hierarchy, new
Core contract, or second Prompting probe file is part of this increment.

## Consequences

FoundationProbe now demonstrates the existing Prompt Budget -> Prompt
Composition executable chain without expanding Core architecture.

This commit is not permission to continue with mechanical 0094/0095 probe
growth. The next decision is a product-value gate. Unless end-to-end
Game/Host/provider integration exposes a concrete missing Core capability,
development should return to AI-Sandbox-Detective and advance the versioned
conversation request / Host bridge path.
