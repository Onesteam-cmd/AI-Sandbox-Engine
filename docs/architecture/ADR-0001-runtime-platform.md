# ADR-0001: Runtime platform for the simulation core

- **Status:** Accepted
- **Date:** 2026-07-21
- **Decision owners:** Project architecture
- **Scope:** Reusable engine core, not presentation or game-engine rendering

## Context

The project needs a strongly typed, deterministic, testable simulation core with substantial domain logic, event processing, persistence boundaries, AI integration boundaries, and potentially many concurrently simulated entities.

The core must remain usable by more than one game and must not be owned by Unreal Engine, Unity, a specific database, or a specific LLM provider.

## Decision

Implement the reusable simulation core in **C# 14 on .NET 10 LTS**.

Game engines, speech systems, LLM providers, databases, and operating-system integrations will connect through explicit adapter projects outside the core.

The repository uses:

- nullable reference types;
- warnings as errors;
- deterministic builds;
- centrally managed NuGet versions;
- strongly typed domain identifiers;
- event-driven runtime boundaries;
- automated tests before integration work.

## Alternatives considered

### C++ as the complete core

Advantages:

- maximum native control;
- direct affinity with Unreal Engine;
- predictable access to low-level optimization.

Rejected as the default domain-runtime language because it increases memory-safety risk, compile times, implementation cost, and testing friction for a system dominated initially by stateful domain logic rather than rendering or tight numeric kernels.

Native modules remain possible behind adapters if profiling proves they are necessary.

### Rust as the complete core

Advantages:

- strong safety guarantees;
- high native performance;
- explicit concurrency model.

Rejected for the initial foundation because game-engine integration, team workflow, tooling familiarity, and iteration cost are less favorable for this project. Rust remains an option for isolated proven bottlenecks behind a stable interface.

### TypeScript or Python as the authoritative runtime

Advantages:

- rapid prototyping;
- broad AI ecosystem;
- low initial ceremony.

Rejected for the authoritative simulation kernel because the project requires strong compile-time domain boundaries, high-throughput long-lived runtime state, and refactoring safety across many systems.

Python or TypeScript may still be used for tooling, content pipelines, experiments, or external services when appropriate.

## Attempt to disprove the decision

The strongest objection is that a future Unreal-based game would require costly C# interoperability.

Mitigation:

- the simulation core exposes coarse-grained contracts instead of per-frame object calls;
- presentation and navigation remain game-engine responsibilities;
- adapters can use IPC, native hosting, generated bindings, or a service boundary;
- the engine architecture is intentionally independent from rendering.

If future profiling proves the boundary too expensive, hot paths can move to native modules without moving gameplay knowledge into the engine or replacing the state model.

## Consequences

Positive:

- fast implementation and refactoring of complex domain systems;
- strong typing and mature testing tools;
- cross-platform runtime;
- straightforward service, CLI, and headless simulation hosts;
- clear separation from presentation engines.

Negative:

- an explicit integration boundary is required for Unreal Engine;
- native interop and deployment must be designed later;
- extremely hot numeric workloads may require specialized native code.

## Revisit criteria

Reopen this ADR only after a measured prototype demonstrates that .NET prevents an essential requirement and the problem cannot be solved by batching, data-oriented design, native specialization, or a process boundary.
