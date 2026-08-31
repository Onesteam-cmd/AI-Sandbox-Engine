# ADR-0007: Immutable exact-type component registry

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Generic component contracts, storage, mutation, and lifecycle
  consistency

## Context

Entity System defines stable world-lifetime identities and their active or
destroyed lifecycle. It deliberately does not define what data an entity owns.
The engine now needs reusable composition without introducing NPC classes,
inheritance trees, gameplay behavior, rendering objects, or concrete game
semantics.

Components will eventually represent position, needs, memory references,
relationships, inventory data, perception state, health, institutions, and many
other independent concerns. The storage foundation must remain type-safe,
immutable inside World State snapshots, deterministic, and efficient enough for
large procedural initialization.

## Decision

Introduce:

- `IComponent` — immutable data-only marker;
- `ComponentMutationStatus` and `ComponentMutationResult` — explicit typed-store
  mutation outcomes;
- `ComponentPurgeResult` — complete cleanup outcome for one entity;
- `ComponentRegistryBuilder` — efficient single-use world-generation builder;
- `ComponentRegistry` — immutable heterogeneous registry of exact typed stores;
- internal `ComponentStore<TComponent>` — sorted entity IDs and parallel typed
  values.

A valid component type is either:

- a concrete value type; or
- a sealed concrete reference type.

Interfaces, abstract types, open generic types, and unsealed reference types are
rejected. This prevents polymorphic storage ambiguity and keeps every component
lookup tied to one exact runtime type.

Each component type owns an independent store. Within a store:

- entity IDs are sorted deterministically;
- lookup uses binary search;
- component values remain strongly typed;
- add, replace, and remove create new arrays;
- unchanged values return the original store;
- entity-ID views are read-only.

`ComponentRegistry` maps exact `Type` keys to internal typed stores. The runtime
type key is an in-memory dispatch detail, not a persistence schema. Commit 0009
will define stable serialized component type identifiers separately.

## Entity lifecycle integration

New or replaced components may be assigned only when the supplied immutable
`EntityRegistry` reports the target as active.

Removal and `PurgeEntity` do not require active membership. This is intentional:
one authoritative World State transition may first destroy an entity and then
remove every component before committing the new root.

`IsConsistentWith(EntityRegistry)` verifies that every stored component owner is
currently active. World roots and later validation systems can use it as an
invariant check.

Component Registry does not modify Entity Registry automatically. Entity
lifecycle and component cleanup are composed explicitly inside one World State
transition, keeping authority and failure semantics visible.

## Initialization

Repeated immutable insertion would become quadratic during large procedural
world creation. `ComponentRegistryBuilder` therefore captures one immutable
entity-registry snapshot, collects assignments in mutable type-specific build
tables, validates duplicates and activity, sorts once per component type, and
freezes into the immutable registry.

The builder is single-use and is not part of authoritative runtime mutation.

## Separation from behavior

Components are data. They do not:

- execute simulation logic;
- call services;
- dispatch events;
- mutate World State;
- generate IDs;
- read wall-clock time;
- schedule work;
- know whether an entity is an NPC, item, location, or institution.

Simulation Systems in later commits will read component snapshots and propose
new component registries through World State transitions.

## Alternatives considered

### Object-oriented entity classes

A class hierarchy such as `NpcEntity`, `ItemEntity`, and `LocationEntity` would
couple identity, storage, and behavior to current game categories. Rejected.

### Public `ComponentStore<T>` properties on the world root

Strongly typed but requires changing the root type for every new component and
does not support generic systems or procedural component registration.
Rejected.

### Dictionary from entity ID to object

Simple heterogeneous storage, but loses compile-time component type safety,
boxes value components, and makes invalid casts a runtime concern. Rejected.

### Mutable archetype ECS immediately

Potentially excellent iteration performance, but it requires chunk ownership,
structural-change scheduling, queries, system execution rules, and mutable
runtime leases before the scheduler exists. Premature for the immutable
authoritative foundation. Deferred.

### External immutable collection package

Could simplify structural sharing, but adds a foundational dependency before
profiling establishes a need. Rejected for now.

### Automatic component cleanup inside Entity Registry

Would make Entity System depend on Component System and reverse the layer order.
Rejected. Explicit World State composition preserves dependency direction.

## Attempt to disprove the decision

Array copying makes frequent runtime component updates linear in the number of
entities carrying that component. This is not expected to be the final hot-loop
representation for all simulation workloads. It is, however, simple,
deterministic, dependency-free, and correct for immutable authoritative
snapshots.

Later profiling may justify chunked copy-on-write stores, persistent trees, or a
mutable scheduler-owned working set that produces immutable commit snapshots.
The public API exposes no arrays and identifies stores only by exact component
type, so representation can change without changing component semantics.

Using CLR `Type` as an in-memory key does not survive renaming as a persistence
identity. The architecture explicitly rejects using it for serialization.
Persistence will require stable schema IDs and migrations.

Deep component immutability remains a contract. Sealed reference types can still
contain mutable nested objects. Future component definitions and validation
tests must preserve immutable object graphs.

## Consequences

Positive:

- exact compile-time component access;
- independent composition of reusable data concerns;
- immutable integration with World State;
- deterministic entity ordering;
- efficient initial build path;
- explicit lifecycle consistency validation;
- no gameplay, event, service, or scheduler coupling;
- internal representation can evolve later.

Negative:

- individual mutations copy one typed store;
- callers must compose entity destruction and component purge explicitly;
- deep immutability cannot be proven mechanically;
- runtime `Type` keys require separate persistence schema IDs;
- multi-component queries and system scheduling are not introduced yet.

## Enforcement

Tests verify value and sealed-reference components, exact-type separation,
deterministic sorting, large initial builds, duplicate and inactive-entity
rejection, null rejection, add, replace, unchanged, remove, purge, immutable
previous snapshots, read-only ID views, consistency detection, and atomic
composition with Entity Registry inside World State.

Repository verification enforces exact typed stores, active-entity assignment,
single-use building, deterministic binary-search storage, unchanged-value
detection, purge support, absence of mutable public setters, hidden event
dispatch, time, identity generation, I/O, concurrency, queues, and game-domain
vocabulary.
