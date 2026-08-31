# ADR-0005: Authoritative immutable World State

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Generic authoritative state ownership and atomic transitions

## Context

The constitution defines World State Manager as the single source of truth.
Every later entity, component, simulation, AI, gameplay, persistence, and
presentation system must observe or change the world through one explicit
authority.

Commit 0005 precedes the Entity and Component systems, so this foundation cannot
contain NPCs, items, quests, crimes, economies, or any concrete storage layout.
It must establish ownership and transition semantics while allowing later
systems to define the immutable root state.

The design must also support deterministic simulation, optimistic concurrency,
testing, and eventual persistence without reading wall-clock time or hiding
external work inside an authoritative commit.

## Decision

Introduce:

- `IWorldState` — marker and deep-immutability contract for a complete state
  root;
- `WorldIdKind` — typed world-identity marker;
- `WorldStateVersion` — monotonic strong version value;
- `WorldStateSnapshot<TState>` — immutable world ID, version, simulation tick,
  and root state;
- `IWorldStateTransition<TState>` — proposal evaluator;
- `WorldStateTransitionDecision<TState>` — accepted next state or rejection;
- `WorldStateApplyStatus` and `WorldStateApplyResult<TState>` — explicit outcome;
- `WorldStateManager<TState>` — single authoritative owner and commit boundary.

A manager begins at version zero. The caller supplies a non-empty world ID,
immutable initial state, and logical simulation tick.

For each transition attempt:

1. capture the current snapshot under the manager lock;
2. reject immediately if the expected version differs or the tick regresses;
3. evaluate the transition exactly once outside the lock;
4. reject without mutation when the transition declines;
5. reacquire the lock;
6. recheck that the captured version is still authoritative;
7. recheck logical time;
8. create a new immutable snapshot with version incremented exactly once;
9. replace the authoritative snapshot atomically.

The manager never mutates an existing state object. Transition implementations
construct a new immutable root. Deep immutability cannot be mechanically proven
for arbitrary object graphs, so it is a documented and tested subsystem
contract.

Transition evaluation runs outside the commit lock. This avoids executing
arbitrary domain logic while holding the authority lock. A transition can lose
the second version check and receive `VersionConflict`; the manager does not
retry or reevaluate it.

## Separation from the event system

World State Manager does not dispatch events itself. A successful commit returns
the new authoritative snapshot. A later runtime orchestration layer can derive
and publish validated events with explicit IDs and ordering after the commit.

This keeps:

- World State as the sole authority;
- events as immutable notifications;
- dispatch as an explicit runtime action;
- persistence as a separate infrastructure concern.

## Alternatives considered

### Mutable object graph behind a manager

Simple initially, but readers can observe partial mutation and external
references can bypass authority. Rejected.

### Generic key-value dictionary as World State

Would be flexible but weakens type safety, makes invariants cross-key concerns,
and prematurely chooses a storage model before Entity and Component design.
Rejected.

### Execute transition logic while holding the lock

Would make the version check simple, but arbitrary game logic could block all
reads, reenter the manager, or call external services while authority is locked.
Rejected.

### Automatic optimistic retries

Convenient for callers, but transition logic could be nondeterministic or have
side effects. Hidden reevaluation would violate exactly-once proposal semantics.
Rejected.

### Event sourcing as the source of truth

A durable event log may be useful later, but adopting event sourcing before
persistence and replay requirements are proven would replace the constitutional
World State authority. Rejected for the foundation.

### Actor or channel-owned World State

A single-threaded mailbox can serialize changes, but it introduces a scheduler,
queue, cancellation lifecycle, and asynchronous ownership before Commit 0008.
Deferred.

### Copy-on-write collection package

Immutable collection packages may become useful for Entity and Component stores,
but choosing them now would add a dependency and constrain storage layout before
measurement. Deferred.

## Attempt to disprove the decision

The root state is a reference type, and the runtime cannot prove its entire
object graph is immutable. This is the main weakness. Restricting the root to
records or a specific immutable collection library would not prove the
immutability of nested objects and would prematurely constrain later systems.
The explicit contract, sealed snapshots, absence of setters, and future
component-store tests provide a better staged boundary.

Optimistic evaluation can waste work when concurrent commits race. The
authoritative simulation scheduler is expected to serialize most writes later;
the second version check still protects correctness now. If measured contention
becomes significant, the runtime can coordinate proposals without changing the
World State contracts.

Returning the snapshot object exposes the immutable root by reference. This is
safe only while implementations honor deep immutability. If future profiling or
integration demonstrates unavoidable mutable state, the architecture must add
controlled leases or snapshot copying rather than silently weakening authority.

## Consequences

Positive:

- one explicit authoritative owner per world;
- atomic versioned commits;
- readers never observe a partially applied transition;
- deterministic logical ticks and no wall-clock dependency;
- exactly-once transition evaluation;
- no hidden retries, event dispatch, persistence, or external I/O;
- Entity and Component systems can define storage later;
- conflicts and domain rejections are explicit outcomes.

Negative:

- deep immutability is a contract rather than a runtime proof;
- rejected or conflicted transitions may allocate proposed state before losing
  the final version check;
- callers must handle version conflicts explicitly;
- event publication remains a separate orchestration step;
- no multi-world transaction or multi-transition batch exists yet.

## Enforcement

Tests verify initialization, typed world IDs, versions, successful commits,
domain rejection, tick regression, transition exceptions, exactly-once
evaluation, and a same-version concurrency race where at most one proposal can
commit.

Repository verification enforces the immutable public surface, explicit
synchronization boundary, expected-version checks, second atomic version check,
single version increment, absence of hidden event dispatch, absence of
wall-clock time, randomness, queues, retries, persistence, networking, and
gameplay-specific vocabulary.
