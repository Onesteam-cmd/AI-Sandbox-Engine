# AI Sandbox Engine Project Constitution

**Status:** CORE FOUNDATION COMPLETE

## 1. Project goal

Build a reusable AI Sandbox Engine for believable human-society simulation and
emergent gameplay. Individual games are consumers of the engine and must not
shape generic Core contracts around one scenario.

## 2. Core principles

- The engine comes before any individual game.
- Emergent behaviour is preferred over scripted behaviour where the product
  benefits from simulation.
- Major systems must remain reusable and presentation-independent.
- Gameplay modules depend on the engine; the engine never depends on gameplay.
- Semantic model integration must not rely on keyword or hotword architecture.
- Foundation changes require a concrete correctness or integration reason.

## 3. Architecture

Conceptual order:

```text
Infrastructure
    ↓
Data
    ↓
Runtime
    ↓
Simulation
    ↓
AI-facing contracts
    ↓
Gameplay
    ↓
Presentation
```

World State is the single source of objective authority.

Model output never modifies the world directly.

```text
Intent / external input
    → Context Retrieval
    → Prompt Composition
    → Model boundary
    → Structured output
    → Validated Action
    → World State
```

## 4. Subjective state

Objective facts and subjective character state are separate concepts.

Core models independently represent:

- Perception;
- Knowledge;
- Memory;
- Beliefs at the consumer/domain layer where applicable;
- Relationships.

Subjective observations do not become objective truth automatically.

## 5. Engine rules

- Strong typing for identity and contracts.
- Immutable authority records where practical.
- Event-driven post-commit integration.
- Explicit deterministic randomness and logical time.
- World State remains authoritative.
- Provider-specific code belongs outside generic Core.
- No gameplay knowledge inside reusable engine systems.

## 6. Development policy

Changes are implemented in bounded, reviewable commits.

```text
Implement → Verify → Build → Test → Commit
```

Architectural decisions are documented through ADRs. A frozen foundation rule
may change only when a concrete limitation, correctness defect, or integration
requirement demonstrates that the existing design is insufficient.

Repository verification and automated tests are part of the acceptance gate for
Core changes.

## 7. Current state

The Core-first roadmap ended at:

```text
0094 Core Product Pipeline Completion
```

The product-shaped FoundationProbe demonstrates Context Retrieval → Prompt
Composition → provider-neutral Model Invocation → Structured Output → Action
Validation → Runtime Command while preserving World State authority until the
final accepted command.

Further Core development is demand-driven by real Game/Host integration rather
than by mechanical expansion of the contract hierarchy.

The first game integration is maintained in the separate `AI-Sandbox-Detective`
Unreal Engine project.
