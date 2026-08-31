# ADR-0019: Directed exact-type relationship state

- **Status:** Accepted
- **Date:** 2026-07-22
- **Scope:** Directed subjective relationships, exact payload types, compact
  change provenance, optimistic revisions, deterministic persistence, and
  separation from knowledge, memory, events, and behavior

## Context

The engine now separates objective World State, perception, current subjective
knowledge, and retained episodic memory. It still needs a current social layer
that answers a different question: how does one entity presently relate to one
other entity?

A relationship is not an objective fact about both parties. It is subjective,
directed, and potentially asymmetric. The state held by A toward B may differ
from the state held by B toward A, and either direction may be absent.

The reusable Core must support detective, Living World, and future modules
without freezing one universal list such as trust, love, fear, hostility, or
loyalty.

## Decision

Introduce:

- `IRelationshipState` as the marker for exact immutable relationship payloads;
- `RelationshipChangeIdKind` for externally assigned change-reference IDs;
- `RelationshipChangeKind` for broad provenance categories;
- `RelationshipChangeReference` for compact latest-change provenance;
- positive-revision `RelationshipEntry<TState>`;
- owner/world-scoped `RelationshipSet<TState>` components;
- explicit add, revise, unchanged, conflict, mismatch, regression, remove, and
  missing outcomes;
- deterministic target-ID ordering and validated restoration.

```text
Knowledge / Memory / Perception / Interaction / External input
                         ↓ explicit command or simulation system
Relationship Set<TState>
owner-scoped current directed state
                         ↓ read-only retrieval
future reasoning, dialogue, and behavior
```

## Exact payloads instead of canonical axes

Core does not define social axes. Concrete modules define immutable payload
types appropriate to their domain and version them through normal persistence
schema evolution.

Examples outside Core might include a compact interpersonal appraisal, a
professional standing model, a faction attitude model, or a family-specific
state. Multiple exact payload types may coexist as separate components for the
same owner.

This preserves compile-time contracts and avoids string-keyed dimension maps.
It also avoids claiming that one fixed axis set is culturally, narratively, or
mechanically sufficient for every game.

## Directed identity

A relationship entry is keyed by target entity ID inside a set attached to one
owner entity:

```text
(owner entity, exact payload type, target entity)
```

No reverse entry is created or revised automatically. A change from A toward B
has no implicit effect on B toward A.

The target does not have to be an active component owner at value-construction
time. Lifecycle and access policy belong to authoritative orchestration. This
also permits concrete games to retain relationships toward absent or destroyed
entities when appropriate.

## Current state and revision

Add establishes revision 1. Revise and remove require the exact expected current
revision. Successful revision increments with checked arithmetic.

Older World State version or logical-tick metadata is rejected as temporal
regression. Identical payload and identical latest change produce `Unchanged`.
The set never retries, queues, or mutates authority directly.

Absence means that no current relationship of that exact payload type exists.
A domain that needs an explicit neutral relationship must represent neutral in
its payload rather than relying on absence.

## Change provenance

Every successful add, revise, or remove attempt carries one explicit
`RelationshipChangeReference`.

Broad kinds are:

- interaction;
- communication;
- perception;
- knowledge;
- memory;
- inference;
- external.

Typed factories preserve perception stimulus/channel IDs, knowledge
claim/evidence IDs, and memory/origin IDs. Communication requires a source
entity. Owner, target, world, version, and tick are always explicit.

Only compact latest provenance is retained in current relationship state.
Narrative history and repeated causes belong to completed events and episodic
memory. Relationship state therefore does not become a second event log.

## Separation from adjacent layers

A relationship mutation does not automatically:

- add or revise knowledge;
- encode or reinforce memory;
- dispatch an event;
- select behavior;
- invoke an LLM or provider;
- apply itself to World State.

Commands or simulation systems interpret inputs and atomically replace the
immutable relationship component through the existing authority boundary.

## Persistence

Restore validates:

- concrete exact relationship payload types;
- positive revisions;
- non-empty owner, target, world, and change IDs;
- owner and target inequality;
- owner/world/target consistency;
- establishment before or at latest update;
- latest change metadata equal to latest update metadata;
- unique targets;
- deterministic target-ID ordering.

Concrete modules serialize their own relationship payloads. Save/restore
continuation must remain byte-identical.

## Alternatives considered

### Fixed canonical trust/love/fear axes

This is easy to query but freezes premature game and cultural assumptions into
generic Core. It also encourages every social mechanic to overload the same few
numbers. Rejected.

### Generic string-keyed dimension dictionary

This is flexible at runtime but loses compile-time contracts, makes schema
validation stringly typed, complicates deterministic migrations, and permits
misspelled dimensions to become data. Rejected.

### One universal non-generic relationship payload

This merely hides a fixed schema behind an object or dictionary and weakens
exact component routing. Rejected.

### Store full relationship history inside the current entry

This duplicates Event and Memory responsibilities, grows current-state payloads
without bound, and makes ordinary retrieval expensive. Rejected.

### Automatically mirror A toward B into B toward A

This destroys subjectivity and prevents asymmetric affection, fear, obligation,
misunderstanding, and deception. Rejected.

### Automatically derive relationships from knowledge or memory

The same evidence may be interpreted differently by different characters and
games. Hidden coupling would bypass command validation and World State
authority. Rejected.

### Automatic decay from wall-clock time

Wall-clock reads violate deterministic simulation. Even simulation-time decay
requires domain policy and therefore remains an explicit future system.
Rejected for this increment.

## Attempt to disprove the decision

Exact payload types can fragment the social model if modules create many
overlapping schemas. That risk is real. Composition guidance and later context
retrievers must define which payload types are canonical for each product.

Latest-only provenance cannot explain a long relationship history by itself.
That is intentional, but it makes Memory/Event retention mandatory for rich
explanations. Duplicating history here would be worse because it would create
two competing histories.

Target lifecycle is not validated inside `RelationshipSet<TState>`. This permits
historical relationships but means an authoritative handler must decide whether
a new relationship toward an unknown or destroyed target is legal.

The Core cannot prove that a payload is deeply immutable. Exact type policy
requires a value type or sealed reference type, matching the existing component,
knowledge, and memory boundaries. Code review and module tests remain necessary
for nested mutable objects.

## Consequences

Positive:

- directed asymmetric social state is explicit;
- no generic game-specific social axes are frozen;
- exact payload types remain compile-time and persistence-safe;
- latest causal provenance is available without duplicating history;
- stale revisions and metadata cannot overwrite current state;
- ordering and save/restore continuation are deterministic;
- Knowledge, Memory, Event, behavior, and provider boundaries remain intact.

Negative:

- product modules must define and govern payload schemas;
- cross-type social aggregation is deferred to context retrieval;
- rich historical explanations require Event and Memory data;
- lifecycle legality remains an orchestration concern;
- no automatic decay or appraisal policy exists.

## Deferred

Commit 0019 does not implement:

- canonical product relationship payloads;
- emotional appraisal;
- simulation-time decay;
- social graph traversal or influence propagation;
- group, household, organization, or faction membership semantics;
- contradiction resolution across payload types;
- behavior selection;
- context retrieval or prompt composition;
- provider calls;
- relationship history beyond latest provenance.

## Enforcement

Tests cover exact payload types, directed asymmetry, typed provenance,
owner/world/target scope, optimistic revisions, temporal regression, explicit
removal, deterministic restore ordering, absence of implicit Knowledge or
Memory mutation, and byte-identical save/restore continuation.

Repository verification enforces the exact component boundary, explicit
statuses, latest-change metadata, absence of fixed social axes, and absence of
hidden clocks, tasks, providers, I/O, authority mutation, event dispatch, or
game-specific vocabulary.
