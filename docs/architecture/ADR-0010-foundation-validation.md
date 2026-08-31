# ADR-0010: Freeze the first foundation after integrated validation

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Cross-layer validation, deterministic continuation, headless proof,
  and broad performance guardrail

## Context

Commits 0001–0009 introduced the dependency-free foundation in isolated,
verified increments:

- repository and toolchain policy;
- Core boundary;
- strongly typed identity;
- immutable events;
- authoritative World State;
- entity lifecycle;
- exact-type components;
- deterministic simulation scheduling;
- versioned snapshot persistence.

Passing isolated unit tests is necessary but not sufficient. The architecture
must now prove that all layers compose without hidden authority, ordering,
identity, or persistence contradictions.

The constitution explicitly defines Commit 0010 as cross-system tests,
determinism tests, architecture tests, performance baselines, and a minimal
headless simulation proof.

## Decision

Commit 0010 adds no new production API. It validates and freezes the existing
foundation through three mechanisms.

### Dedicated cross-system tests

`FoundationValidationTests` constructs a small immutable world containing:

- stable world, entity, system, and event IDs;
- active and later destroyed entities;
- typed components;
- two ordered simulation systems;
- one authoritative World State Manager;
- deterministic snapshot capture and restore;
- post-commit event dispatch.

The primary test compares:

1. eight uninterrupted logical ticks;
2. two ticks, snapshot capture, restore, and six more ticks.

The final snapshot metadata, encoded payload bytes, and SHA-256 checksum must be
identical.

The scenario also proves that:

- a destroyed ID remains reserved after restoration;
- destroyed entities retain no components;
- Entity and Component registries remain consistent;
- event dispatch occurs after commit and cannot mutate World State;
- every successful tick advances version and logical time exactly once;
- restored snapshots can be continued and persisted again.

### Minimal headless executable

`AI.Sandbox.Engine.FoundationProbe` is an executable sample, not a production
dependency. It creates one entity with one typed component, runs deterministic
simulation, persists and restores at the midpoint, compares the uninterrupted
and resumed results, dispatches a post-commit event, and emits a stable checksum.

The probe uses no game engine, UI, file storage, database, network, LLM, clock
inside Core, or external service.

### Repeatable validation entry point

`eng/validate-foundation.ps1`:

1. verifies repository architecture;
2. optionally builds Release;
3. runs the dedicated integration tests;
4. runs the headless probe repeatedly;
5. requires the same checksum every repetition;
6. enforces a deliberately broad elapsed-time ceiling.

The time ceiling is a regression tripwire, not a microbenchmark. It catches
catastrophic accidental complexity, deadlocks, or unbounded work while avoiding
hardware-specific optimization claims.

## Performance baseline policy

The initial default validation workload is:

- 5,000 logical ticks per probe;
- three independent repetitions;
- identical final SHA-256 checksum;
- each repetition below 30 seconds.

This is intentionally loose. Commit 0010 does not claim the immutable component
representation is production-optimal. Detailed profiling and workload-specific
budgets require representative simulation systems and world sizes.

The baseline may be tightened only with measurements on documented hardware and
must not make CI dependent on unstable microsecond timing.

## Architecture freeze

The first foundation is accepted when:

- all ordinary tests pass;
- all cross-system tests pass;
- repeated headless probes produce one checksum;
- the broad performance guardrail passes;
- repository architecture verification passes;
- Release build is clean;
- the commit leaves a clean working tree.

Future work may extend the engine but cannot silently weaken:

- World State authority;
- immutable event facts;
- stable world-lifetime entity identity;
- exact component-type semantics;
- one deterministic system sequence per tick;
- one atomic World State commit per tick;
- stable persistence schema identity;
- checksum-before-decode restoration;
- no direct LLM mutation of World State.

Any required foundation change needs a new ADR, migration impact analysis, and
updated validation scenario.

## Alternatives considered

### Continue directly into AI systems

Faster in the short term, but foundation integration defects would become
entangled with memory, behavior, and provider code. Rejected.

### Add a benchmarking package

Provides stronger statistics but adds a dependency and creates optimization
pressure before representative workloads exist. Rejected for this checkpoint.

### Use only unit tests

Would not prove save/restore continuation, post-commit event ordering, or
cross-layer deterministic equivalence. Rejected.

### Put the probe inside Core

Would pollute the reusable assembly with example state and validation behavior.
Rejected. The probe remains a sample executable.

### Assert a strict milliseconds-per-tick target

Hardware, virtualization, antivirus, and build conditions vary. A strict target
would be flaky and misleading. Rejected.

## Attempt to disprove the decision

The validation world is intentionally small and cannot prove scalability to
millions of entities or complex social simulation. It proves semantic
composition, not final throughput.

The test codec is specific to the validation state and does not prove future
game codecs are correct. It establishes the required deterministic and
versioned contract they must follow.

A repeated checksum can miss two executions that are deterministically wrong in
the same way. Therefore the tests also assert expected lifecycle, component,
version, tick, and event properties.

The 30-second guardrail is too loose to detect moderate regressions. That is
intentional until representative workloads and hardware baselines exist.

## Consequences

Positive:

- the complete foundation has one executable proof;
- save/restore continuation is compared byte-for-byte;
- construction order cannot alter final snapshots;
- architecture validation has one standard command;
- severe performance regressions and deadlocks have a guardrail;
- no new production abstraction is invented solely for testing;
- future foundation changes have a regression target.

Negative:

- validation scenario code is duplicated between tests and sample at different
  levels of detail;
- performance results are informational rather than optimization-grade;
- game-specific schemas, commands, AI, perception, memory, and behavior remain
  unimplemented.

## Enforcement

Repository verification requires the dedicated tests, sample project, validation
script, solution registration, exact Core-only sample dependency, repeated
checksum validation, performance budget check, and explicit coverage of Events,
World State, Entities, Components, Simulation, and Persistence.
