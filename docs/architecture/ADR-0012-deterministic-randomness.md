# ADR-0012: Explicit immutable deterministic random streams

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Authoritative random state, independent streams, persistence,
  unbiased ranges, and algorithm versioning

## Context

The engine constitution requires authoritative randomness to be explicit.
`System.Random`, `Random.Shared`, wall-clock seeding, and hidden mutable global
generators would make simulation results depend on process lifetime, thread
timing, unrelated call order, or framework implementation changes.

Procedural worlds, NPC decisions, combat, economy, weather, and future behavior
systems will all need randomness. One shared sequence would also make a harmless
new draw in one subsystem change every later result in unrelated systems.

Random state must therefore be immutable World State data, independently
partitioned by stable stream identity, reproducible after save/restore, and
versioned as a persistence contract.

## Decision

Introduce:

- `RandomSeed` — explicitly initialized 64-bit root seed;
- `RandomStreamIdKind` — typed stable identity for one independent stream;
- `RandomAlgorithmVersion` — persisted algorithm contract version;
- `DeterministicRandomState` — immutable complete stream state;
- `RandomDraw<T>` — one value coupled with the exact next state.

Version 1 uses the public-domain SplitMix64 algorithm with frozen constants and
bit operations.

Every draw is pure:

```text
old state
    ↓
sample value + new state
```

The caller must place the returned state into authoritative World State before
using it as the basis for later draws.

## Seed and stream derivation

`RandomSeed.From(ulong)` makes all 64-bit values valid, including zero. The
default struct remains distinguishable and invalid.

`DeterministicRandomState.Create(seed, streamId)` combines:

- the explicit root seed;
- a stable FNV-1a hash of the canonical typed stream ID;
- one fixed version-1 salt;
- the version-1 SplitMix64 finalizer.

The same seed and stream ID always derive the same sequence. Different stream
IDs derive independent states.

Stable stream IDs must represent semantic ownership, for example:

- world generation terrain;
- weather;
- one institution;
- one entity decision domain;
- one procedural location.

They are externally assigned. Randomness code never generates IDs.

## Persisted state

A complete stream state contains:

- algorithm version;
- stream ID;
- internal 64-bit state;
- primitive draw count.

A game codec persists all four values. Restoration rejects unsupported algorithm
versions rather than silently changing the sequence.

The draw count is diagnostic and guards overflow. It is not used to recompute
the stream by replaying previous draws.

## Supported draws

Version 1 provides:

- full-range `ulong`;
- unbiased bounded `ulong`;
- unbiased bounded `int`;
- `double` in `[0,1)` using exactly 53 random bits;
- Boolean.

Bounded integers use rejection sampling. Modulo bias is not accepted.

A bounded draw may consume more than one primitive 64-bit value. The returned
`DrawCount` therefore records actual primitive consumption.

## Integration with World State

Random generators are not services and are not stored in mutable singletons.
They are values inside components or another immutable World State root.

A simulation system:

1. reads one stream state;
2. draws a value;
3. validates and uses the value;
4. writes the returned next state into its proposed immutable state;
5. commits the complete tick through World State Manager.

A rejected tick, command conflict, or exception does not commit the proposed
random state, so randomness rolls back with every other state change.

## Commands and external AI

A pure command handler may use explicitly stored random state only when the
command policy permits authoritative randomness. The next state must include the
advanced stream.

External LLM providers never draw authoritative randomness. They can propose
commands, but deterministic validation and random outcomes remain inside the
versioned engine state.

## Alternatives considered

### `System.Random` with a saved integer seed

The framework algorithm is not a stable persistence contract, and a seed alone
cannot restore a partially consumed stream. Rejected.

### One global random generator

Makes unrelated systems sensitive to each other's call count and creates hidden
mutable authority. Rejected.

### Cryptographic randomness

Useful for secrets, not deterministic replay. Rejected for simulation
authority.

### Derive every result directly from tick and entity ID

Excellent for some stateless procedural decisions, but insufficient for
stateful sequences, rejection sampling, and evolving local streams. It may be
added later as a separate deterministic hash primitive. Not selected as the
only model.

### Mutable generator class

Convenient but easy to advance outside World State commits and difficult to roll
back. Rejected.

### Store only draw count and recompute from the seed

Requires replay proportional to stream age or a jump-ahead contract. Rejected.
The complete internal state is small and explicit.

## Attempt to disprove the decision

SplitMix64 is not suitable for cryptography and has a 64-bit state. It is
selected for deterministic simulation, speed, simple frozen semantics, and
excellent statistical behavior for its intended role. Security tokens and
unpredictable secrets require a different subsystem.

FNV-1a stream-ID hashing can theoretically collide. The stream ID remains stored
in state, but two colliding derivations could begin with the same internal state.
This risk is negligible for semantic engine streams but can be replaced in a
future algorithm version if requirements change.

A stream can still be coupled too broadly if a feature reuses one ID for
unrelated decisions. Stream ownership is an architectural responsibility and
must be reviewed with each simulation subsystem.

Repeated immutable component updates currently copy typed stores. The randomness
state itself is only a few words; broader ECS optimization remains separate.

## Consequences

Positive:

- bit-reproducible authoritative randomness;
- exact save/restore continuation;
- rollback naturally includes random state;
- independent subsystem streams;
- explicit algorithm migration boundary;
- no global mutable state;
- unbiased integer ranges;
- stable behavior independent of .NET random implementations;
- no clocks, generated IDs, threads, I/O, or providers.

Negative:

- callers must persist every returned next state correctly;
- stream ownership and IDs require deliberate design;
- version 1 is not cryptographically secure;
- stream-hash collisions are theoretically possible;
- bounded draws can consume a variable number of primitive values.

## Enforcement

Tests verify the known SplitMix64 version-1 vector, explicit zero seed,
same-seed reproduction, different and independent streams, bounded ranges,
double and Boolean conversion, invalid state and overflow behavior, exact state
restoration, and byte-identical Scheduler continuation across save/restore.

Repository verification enforces immutable persisted state fields, explicit
algorithm version, stable stream derivation, checked draw counting, unbiased
rejection sampling, stable 53-bit doubles, and absence of `System.Random`,
clocks, generated IDs, mutable setters, hidden execution, I/O, providers, and
game-domain vocabulary.
