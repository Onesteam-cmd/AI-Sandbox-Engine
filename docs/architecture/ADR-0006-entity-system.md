# ADR-0006: Immutable entity identity and lifecycle registry

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Generic entity identity, existence, and destruction semantics

## Context

World State now provides one authoritative immutable root and atomic transition
boundary. The next layer must establish what it means for an entity to exist
without prematurely introducing components, NPC behavior, inventories, quests,
rendering objects, or game-specific classes.

Entity identifiers can appear in events, memories, beliefs, relationships,
evidence, persistence records, and external integrations long after an entity
ceases to be active. Reusing a destroyed identifier would make historical
references silently point to a different entity.

The system must remain immutable so it can be embedded safely inside a
`WorldStateManager<TState>` root.

## Decision

Introduce:

- `EntityIdKind` — the typed-ID marker for entity identities;
- `EntityLifecycleStatus` — `Unknown`, `Active`, or `Destroyed`;
- `EntityMutationStatus` — explicit outcomes for create and destroy attempts;
- `EntityMutationResult` — the target ID, outcome, resulting registry, and
  whether the registry changed;
- `EntityRegistry` — immutable known and active identity sets.

An entity is currently nothing more than a stable identity with lifecycle state.
Components are introduced separately in Commit 0007.

The registry stores two deterministic sorted identity arrays:

- **known identities** — every identifier ever registered in the world;
- **active identities** — the currently existing subset.

Creating a previously unknown identity inserts it into both sets. Destroying an
active entity removes it only from the active set. Its identity remains in the
known set permanently and cannot be created again.

Queries use binary search. Single lifecycle mutations create new arrays and
return a new registry, leaving previous registries unchanged. Initial world
generation uses `FromActiveEntities`, which materializes the input once, sorts
once, validates uniqueness, and avoids repeated incremental insertion.

The arrays are private and are exposed only through read-only collection
wrappers. Entity Registry does not mutate World State itself. Domain transitions
compose a new registry into their immutable root and submit it through World
State Manager.

## Why destroyed identities remain known

Historical systems need stable referential meaning:

- an event about a dead entity must still refer to that entity;
- an NPC memory must not start referring to a newly spawned object;
- a saved relationship or crime record must remain interpretable;
- network and persistence boundaries must not observe identity reuse.

The foundation therefore treats entity IDs as world-lifetime unique. Destruction
ends activity, not historical identity.

## Separation from components

Entity Registry knows only whether an identity is unknown, active, or destroyed.
It does not know:

- which components an entity has;
- what an entity represents;
- whether it is an NPC, item, location, projectile, or abstract institution;
- how destruction affects components;
- which events should be emitted.

Commit 0007 will define component storage and its relationship to active entity
identities without changing these lifecycle semantics.

## Alternatives considered

### Remove destroyed IDs completely

Memory-efficient, but permits accidental identity reuse and makes historical
references ambiguous. Rejected.

### Mutable hash set inside World State

Fast mutation, but external references and readers could observe changes outside
the authoritative transition boundary. Rejected.

### One dictionary from ID to lifecycle enum

Simple and offers average constant-time lookup, but requires copying a hash table
for immutable changes and carries more per-entry overhead. It also does not
exploit deterministic ordering. Deferred until profiling proves the sorted-array
representation inadequate.

### Dedicated entity classes or inheritance hierarchy

Would mix identity with behavior and game semantics before the Component System
exists. Rejected.

### Entity ID generation inside the registry

Convenient, but hides randomness or time inside state mutation and weakens
deterministic replay. Rejected. Identity generation remains an explicit runtime
dependency.

### Immediate event emission from lifecycle methods

Would couple pure state calculation to runtime ordering and dispatch. Rejected.
A later orchestration layer can derive `EntityCreated` and `EntityDestroyed`
events after an authoritative commit.

## Attempt to disprove the decision

Sorted arrays provide logarithmic queries but linear copying for individual
create and destroy operations. Large procedural generation would become
quadratic if it repeatedly called `CreateEntity`; the batch factory exists
specifically to avoid that path.

A long-running world retains tombstones for every destroyed identity. At one
million destroyed entities, raw GUID storage is material but still bounded and
predictable. Persistence or archival layers may later compress historical
identity data, but they must preserve the no-reuse invariant.

If profiling shows frequent high-volume lifecycle churn, the private
representation can change to chunked immutable storage, a persistent tree, or a
specialized table. The public API exposes no array representation, so such a
change does not alter lifecycle semantics.

## Consequences

Positive:

- stable world-lifetime entity identity;
- no accidental resurrection or ID reuse;
- immutable composition with World State;
- deterministic ordering;
- explicit non-exceptional lifecycle outcomes;
- no component, gameplay, event-dispatch, or generator coupling;
- efficient batch construction for initial world generation.

Negative:

- individual lifecycle mutations copy arrays;
- destroyed identity tombstones accumulate;
- restoring mixed active and destroyed state is deferred to Persistence;
- component cleanup and event generation remain separate responsibilities.

## Enforcement

Tests verify deterministic ordering, single enumeration of batch input, duplicate
and empty-ID rejection, immutable create and destroy behavior, permanent
tombstones, no recreation after destruction, read-only views, large initial
batch construction, and composition inside an authoritative World State
transition.

Repository verification enforces the known/active split, permanent retention on
destroy, explicit mutation results, no mutable setters, no identity generation,
no wall-clock access, no event dispatch, and no component, gameplay, or AI
vocabulary.
