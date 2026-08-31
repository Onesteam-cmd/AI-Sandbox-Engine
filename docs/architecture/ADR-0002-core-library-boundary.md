# ADR-0002: Dependency-free core library boundary

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Lowest reusable engine assembly

## Context

The engine needs a stable lowest-level assembly for foundational types and contracts. If this assembly acquires provider SDKs, gameplay code, storage implementations, or presentation dependencies, all higher layers inherit those constraints and the architecture becomes difficult to reuse.

## Decision

Create `AI.Sandbox.Engine.Core` as a dependency-free .NET class library.

The assembly may contain only foundational reusable primitives and contracts that are valid for both the detective game and Living World. It has:

- no NuGet package references;
- no project references;
- no provider implementations;
- no gameplay concepts;
- no game-engine types;
- no domain system implementation in Commit 0002.

Its first public API will be strongly typed identifiers in Commit 0003. Commit 0002 intentionally exposes no public types.

The paired `AI.Sandbox.Engine.Core.Tests` project may depend on the core assembly and test-platform packages only.

## Alternatives considered

### One large engine project

This minimizes the initial project count, but allows unrelated concerns to couple silently and makes dependency direction unenforceable. Rejected.

### Separate project for every future concept immediately

This maximizes theoretical isolation but creates premature boundaries before behavior and change patterns are known. Rejected for now.

### Core depending on utility packages

Convenient libraries could reduce small amounts of implementation work, but every future engine layer would inherit their API, versioning, allocation, and deployment constraints. Rejected unless a later measured requirement justifies a new ADR.

## Attempt to disprove the decision

A dependency-free core may require implementing small primitives that a package already provides. That cost is acceptable because the core is deliberately narrow. Complex features remain outside it, and native framework APIs remain available.

The boundary would be invalid if foundational engine behavior required a third-party runtime dependency that could not be isolated above the core. No such requirement currently exists.

## Consequences

Positive:

- stable dependency direction;
- minimal deployment surface;
- straightforward testing and reuse;
- no contamination by LLM, persistence, speech, or game-engine providers.

Negative:

- some low-level primitives must be implemented locally;
- moving a type into the core requires stronger scrutiny;
- public APIs introduced here become expensive to change.

## Enforcement

Repository verification rejects package or project references from `AI.Sandbox.Engine.Core`. Automated tests verify assembly identity, absence of premature public API, and framework-only runtime references.
