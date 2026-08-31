# ADR-0008: Deterministic caller-driven simulation scheduler

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Logical tick ownership, system ordering, atomic tick commits, and
  conflict semantics

## Context

The engine now has authoritative World State, stable entity lifecycle, and
immutable component storage. Behavior still has no execution model. A scheduler
must define when simulation logic runs, which state each system sees, how
systems compose, and when a complete tick becomes authoritative.

The foundation must remain deterministic and independent of rendering frame
rate, wall-clock time, background threads, timers, game engines, LLM calls, and
external I/O.

## Decision

Introduce:

- `SimulationSystemIdKind` — stable typed identity for each registered system;
- `ISimulationSystem<TState>` — synchronous side-effect-free system contract;
- `SimulationSystemContext<TState>` — immutable tick, version, world, order, and
  working-state context;
- `SimulationSystemDecision<TState>` — unchanged, updated, or rejected result;
- `SimulationSchedulerBuilder<TState>` — single-use fixed-order registration;
- `SimulationScheduler<TState>` — caller-driven logical tick coordinator;
- `SimulationStepResult<TState>` — applied, conflict, or rejection diagnostics.

One `RunNextTick()` call means:

1. serialize calls on this scheduler instance;
2. read one authoritative World State snapshot;
3. calculate exactly `previousTick + 1` using checked arithmetic;
4. create one pure World State transition;
5. execute every registered system once in registration order;
6. pass each system the working state produced by earlier systems;
7. abort the complete tick if any system rejects or throws;
8. attempt exactly one authoritative World State commit;
9. return explicit diagnostics without automatic retry.

A scheduler with zero systems still advances one logical tick and one World State
version. This permits time to progress before concrete simulation systems exist.

## Atomicity

All system updates within one tick are local proposals until the final World
State commit. Readers cannot observe an intermediate state between systems.

If a system rejects, the transition returns a World State rejection and the
authoritative snapshot remains unchanged. Later systems do not execute.

If a system throws or returns an invalid decision, the exception escapes and
authoritative state remains unchanged.

If another writer commits after system evaluation but before this scheduler can
commit, the result is `VersionConflict`. Systems are not executed again. Hidden
optimistic retries are forbidden because systems may be computationally
expensive and exactly-once evaluation is easier to reason about.

## Ordering and identity

Registration order is the deterministic system order. Every system also receives
a stable externally supplied `Id<SimulationSystemIdKind>` for diagnostics,
replay metadata, configuration, and future event attribution.

CLR type name is not used as system identity. Multiple configured instances of
the same system type are allowed when they have distinct IDs.

## Time model

The scheduler owns logical tick advancement, not real-time pacing. It does not
know how many seconds a tick represents and does not sleep.

A future host or engine adapter may call the scheduler:

- once per fixed update;
- multiple times to catch up;
- manually in tests;
- as fast as possible for offline simulation;
- according to a separate pacing policy.

Rendering frames and wall-clock duration therefore cannot alter authoritative
system order or tick arithmetic.

## Concurrency

Calls to one scheduler instance are serialized by an explicit local lock. This
prevents the same system instances from executing concurrently through that
scheduler and ensures concurrent callers receive distinct successive ticks.

World State Manager remains independently authoritative. External writers can
still race with the scheduler, producing an explicit version conflict. The
scheduler does not suppress or retry that conflict.

## Separation from asynchronous AI and external work

Authoritative simulation systems are synchronous and side-effect-free. They must
not await an LLM, network service, database, speech provider, or game-engine
object.

External AI work will later operate through a separate pipeline:

1. capture a versioned subjective context;
2. call the provider outside authoritative simulation;
3. validate the structured response;
4. submit typed commands or intents;
5. let deterministic simulation systems validate and apply them.

This prevents provider latency or nondeterminism from entering the tick lock.

## Separation from events

The scheduler does not dispatch events while systems execute or while World State
is being committed. Events remain immutable post-commit notifications.

A later runtime orchestration layer will derive event envelopes after a
successful tick, assign explicit event IDs and sequence numbers, and dispatch
them separately.

## Alternatives considered

### One World State commit per system

Simple to inspect, but exposes partial ticks, increments versions repeatedly,
and allows readers to observe states that never represent a complete simulation
step. Rejected.

### Parallel system execution

Potentially faster, but requires dependency graphs, conflict detection, merging,
and deterministic reduction semantics. Premature and dangerous before profiling.
Rejected for the authoritative foundation.

### Timer-owned scheduler

Convenient for a standalone process, but couples logical simulation to
wall-clock pacing and creates lifecycle and threading concerns inside Core.
Rejected.

### Asynchronous system contract

Would permit external calls during simulation, weakening atomicity and
determinism. Rejected. Asynchronous providers belong outside authoritative tick
evaluation.

### Automatic version-conflict retries

Could hide transient contention, but would execute systems more than once and
make side-effect mistakes harder to detect. Rejected.

### Order inferred from system types or dependencies

Reflection-based discovery is less explicit and can change when assemblies or
names change. Rejected. Registration order is visible and testable.

## Attempt to disprove the decision

Sequential system execution limits CPU parallelism. That cost is accepted until
real profiles identify hot independent workloads. Later execution plans may
parallelize read-only computations and merge typed proposals, but they must
preserve deterministic ordering and one atomic tick commit.

Every system creates immutable states, which can cause allocation and copying.
Commit 0007 already documents that current component storage is a correctness
foundation, not necessarily the final hot-loop representation. A future
scheduler-owned mutable working set could produce immutable commit snapshots,
but only after ownership and rollback semantics are proven.

The scheduler lock serializes potentially long system evaluation. It protects
one scheduler's system instances, not World State readers. If systems become
slow, the correct response is to optimize or partition pure simulation work,
not to move external I/O inside the tick.

## Consequences

Positive:

- deterministic fixed system order;
- one logical tick equals one atomic World State commit;
- no partial authoritative tick states;
- explicit rejection and conflict behavior;
- no hidden retries, timers, threads, I/O, or provider calls;
- rendering and wall-clock pacing remain external;
- concurrent calls on one scheduler produce distinct ticks;
- zero-system worlds can still advance logical time.

Negative:

- systems execute sequentially;
- immutable intermediate states may allocate;
- external writers can cause unretried conflicts;
- events and commands remain separate future orchestration layers;
- no per-system cadence, dependency graph, batching, or parallel plan exists yet.

## Enforcement

Tests verify registration order, stable system metadata, unchanged and updated
decisions, whole-tick rejection, exception rollback, null-decision failure,
zero-system advancement, tick overflow, external version conflict without
retry, and serialization of concurrent scheduler calls into distinct ticks.

Repository verification enforces explicit system order, unique typed IDs,
single-use construction, one next-tick calculation, World State-only commit,
system execution counting, one final accepted state, and absence of hidden
threads, timers, parallelism, queues, wall-clock time, ID generation, event
dispatch, persistence, I/O, and game-domain vocabulary.
