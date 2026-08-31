# ADR-0018: Explicit episodic retention and deterministic recall

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Memory episodes, provenance, strength, salience, reinforcement,
  weakening, forgetting, deterministic recall, persistence, and separation from
  current knowledge

## Context

Knowledge represents what an entity currently accepts as claims. It deliberately
does not retain the history of experiences, earlier revisions, or forgotten
information.

The engine needs an episodic layer that can preserve selected perceptions,
communications, knowledge states, and authored experiences without turning
every observation into permanent knowledge or introducing wall-clock timers.

## Decision

Introduce:

- `IMemoryContent` for exact immutable episode payloads;
- typed memory and origin IDs;
- perception, knowledge, communication, and external origin kinds;
- `MemoryOriginReference` with owner, world, version, tick, and source
  provenance;
- integer `MemoryStrength` and `MemorySalience`;
- positive-revision `MemoryEntry<TContent>`;
- owner-scoped `MemoryStore<TContent>` components;
- explicit encode, reinforce, weaken, forget, and remove outcomes;
- bounded deterministic recall queries and read-only ranked results.

## Semantic boundary

```text
Perception / communication / knowledge
current evidence or claim
        ↓ explicit command or simulation system
Memory Entry
retained episode
        ↓ read-only recall
candidate context for future reasoning and behavior
```

Memory does not replace current Knowledge Set. A remembered claim may no longer
be currently accepted, and current knowledge may exist without an episodic
memory.

Encoding a knowledge entry into memory is explicit. Producing an observation or
knowledge revision does not create a memory automatically.

## Strength and salience

Strength models retention availability. Salience models importance for recall.

Both use integer basis points. Retained memories require non-zero strength.
Salience may be zero.

Core does not derive either value from emotional state, elapsed real time, or a
provider. Domain systems explicitly choose initial values and update amounts.

## Reinforcement, weakening, and forgetting

Reinforcement and weakening require:

- exact expected revision;
- explicit World State version;
- explicit logical simulation tick;
- integer strength and salience changes.

Older update metadata is rejected as `TemporalRegression`.

If weakening reaches zero strength, the entry is removed with `Forgotten`.
There is no hidden timer, background decay, scheduled task, or wall-clock read.

Explicit `Remove` represents administrative deletion rather than gradual
forgetting.

## Recall

Recall is read-only and does not reinforce memories automatically.

A recall query supplies:

- maximum result count;
- minimum strength;
- minimum salience.

Results are ranked deterministically by:

1. strength plus salience;
2. strength;
3. salience;
4. last updated World State version;
5. last updated logical tick;
6. stable memory ID.

No random sampling, embedding search, LLM call, or mutable access counter is
hidden in Core.

## Origins

Perception origins retain stimulus and channel provenance.

Knowledge origins retain claim and evidence IDs and may carry the source entity
or underlying perception provenance from the current knowledge entry.

Communication origins require a source entity.

External origins represent explicit authored or imported episodes.

Every origin must match the Memory Store owner and world.

## Persistence

Restore validates:

- exact concrete content types;
- positive revisions;
- non-zero retained strength;
- initialized salience;
- origin before or at encoding;
- encoding before or at last update;
- owner and world consistency;
- unique memory IDs;
- deterministic memory-ID ordering.

Concrete modules serialize their own episode payloads.

## Deferred

Commit 0018 does not implement:

- automatic time-based decay;
- emotional appraisal;
- associative graphs;
- semantic embeddings;
- consolidation during sleep;
- false-memory generation;
- memory editing by LLMs;
- context-window retrieval policy;
- relationships or behavior selection.

These remain explicit future layers.

## Consequences

Positive:

- episodic retention is separate from current knowledge;
- forgetting and reinforcement are deterministic and testable;
- provenance survives from perception and knowledge;
- recall is bounded, stable, and read-only;
- no hidden clocks, tasks, providers, or automatic persistence;
- memories can remain after knowledge changes or disappears.

Negative:

- domain systems choose strength and salience;
- no automatic decay exists yet;
- recall ranking is generic rather than semantic;
- content types and serialization remain module responsibilities;
- removed memories leave no tombstone in the current store.

## Enforcement

Tests cover fixed-point values, origin provenance, exact content types,
owner/world scope, deterministic ordering, optimistic revisions, temporal
regression, explicit forgetting, read-only recall ranking, explicit
knowledge-to-memory encoding, and byte-identical save/restore continuation.
