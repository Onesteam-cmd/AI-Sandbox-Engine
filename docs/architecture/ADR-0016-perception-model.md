# ADR-0016: Snapshot-gated subjective perception evaluation

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Candidate stimuli, observers, sensory channels, subjective signals,
  confidence, exact evaluator routing, stale-context handling, and semantic
  separation from facts, knowledge, beliefs, and memory

## Context

The engine now has authoritative state, events, commands, time, space, and a
caller-driven runtime. It still needs a formal boundary between:

- what objectively exists or happened in World State;
- what a particular observer had an opportunity to sense;
- what subjective signal the observer actually receives;
- what the observer later stores as knowledge, belief, or memory.

Without this boundary, gameplay code may grant global omniscience, write true
World State directly into NPC knowledge, or ask an LLM to infer perception from
an unbounded world dump.

Perception must remain deterministic, observer-specific, snapshot-specific, and
free of hidden presentation queries or state mutation.

## Decision

Introduce:

- `IPerceptionStimulus` — immutable candidate sensory input;
- `IPerceptionSignal` — immutable subjective output for one observer;
- `PerceptionStimulusIdKind` — externally assigned provenance identity;
- `PerceptionChannelIdKind` — stable sensory channel identity;
- `PerceptionConfidence` — initialized integer basis-point metadata;
- `PerceptionStimulusEnvelope<TStimulus>` — observer and snapshot-gated input;
- `PerceptionContext<TState,TStimulus>` — immutable evaluator context;
- `IPerceptionEvaluator<TState,TStimulus,TSignal>` — pure exact-pair evaluator;
- `PerceptionDecision<TSignal>` — observed or ignored;
- `PerceptionObservation<TSignal>` — subjective signal plus exact provenance;
- `PerceptionProcessorBuilder<TState>` — exact pair registration;
- `PerceptionProcessor<TState>` — read-only version-gated evaluation;
- explicit evaluation statuses and results.

## Semantic layers

The model freezes the following separation:

```text
World State / Event
objective authoritative state or completed fact
        ↓ candidate generation
Stimulus
something an observer may be able to sense
        ↓ pure evaluator
Observation / Signal
subjective evidence available to that observer
        ↓ future cognition layers
Knowledge / Belief / Memory
observer-owned retained interpretation
```

A stimulus is not automatically perceived.

An observation is not automatically true.

An observation is not automatically remembered.

The Perception Processor never writes knowledge, belief, or memory.

## Snapshot gating

Every stimulus envelope records:

- stimulus ID;
- sensory channel ID;
- observer entity ID;
- intended world ID;
- expected World State version;
- expected logical tick;
- exact stimulus payload.

World, version, and tick mismatches are rejected before evaluator execution.

After the pure evaluator returns, the processor reads authority again. If World
State version or tick changed during evaluation, the candidate result is
discarded as `VersionConflict`. The evaluator is not retried.

This prevents a costly spatial, acoustic, adapter, or model-assisted evaluator
from returning a signal associated with a different world than the one it read.

## Exact evaluator pairs

Evaluators are registered by the exact pair:

```text
stimulus type + signal type
```

The same stimulus type may intentionally produce different signal types through
different registered evaluators. Interfaces, abstract types, open generics, and
unsealed reference types are rejected.

Registration and evaluation use no reflection discovery or naming convention.

## Confidence

`PerceptionConfidence` uses zero through 10,000 integer basis points.

The default struct is invalid. Initialized zero exists but cannot accompany an
observed signal. An observation therefore requires confidence from 1 through
10,000.

Confidence is evaluator metadata, not an objective truth probability. Later
knowledge and belief systems decide how to interpret it.

## Spatial and presentation boundaries

The Perception Model does not implement line of sight, acoustic propagation,
lighting, raycasts, or navigation.

A pure evaluator may use:

- authoritative spatial places and local integer positions;
- dynamic door, wall, material, or obstruction components added later;
- externally supplied deterministic adapter results;
- game-specific sensory policy outside the generic Core contracts.

The future acoustic evaluator must not use one fixed hearing radius. It must
combine at least:

- source intensity, including player and NPC voice loudness;
- distance attenuation;
- wall, door, and material transmission loss;
- observer hearing threshold and modifiers.

Loud speech may remain audible to another observer behind a wall when the
remaining propagated intensity exceeds that observer's threshold. Quiet speech
may fail even at a shorter distance. This policy belongs to the acoustic
evaluator and dynamic world data, not to the generic perception contracts.

For example, a host may perform an Unreal raycast or acoustic query, convert the
result into an immutable candidate stimulus tied to snapshot version `V`, and
submit it. If the world advances before evaluation completes, the result is
discarded.

Presentation queries are evidence sources, never authority.

## Read-only execution

Perception Processor reads World State Manager but never calls `TryApply`.

Observed and ignored results preserve the original immutable snapshot when
authority remains stable. Evaluator exceptions propagate as programming or
adapter failures and cannot partially mutate World State.

No event is dispatched and no observation is persisted automatically.

## Identity and provenance

The processor never generates stimulus, channel, entity, or observation IDs.

`PerceptionObservation<TSignal>` records:

- source stimulus ID;
- channel ID;
- observer ID;
- world ID;
- exact version and tick;
- confidence;
- signal payload.

The future knowledge and memory layers can retain this provenance without
mistaking it for objective truth.

## Alternatives considered

### Give every observer direct World State access

Creates omniscience and bypasses sensory limitations. Rejected.

### Write observations directly into knowledge

Conflates momentary evidence with retained interpreted cognition. Rejected.

### Let the evaluator mutate observer components

Makes failed or stale perception non-atomic and hides cognition policy inside a
query. Rejected.

### Put raycasts and acoustic propagation in Core now

Couples the reusable model to presentation geometry and premature policies.
Rejected.

### Use one non-generic dictionary payload

Loses compile-time contracts and encourages stringly typed signals. Rejected.

### Retry after a version conflict

Could execute expensive or side-effectful adapter code more than once and would
hide stale context. Rejected.

### Treat confidence as floating probability

Introduces floating semantics and falsely implies calibrated probability.
Rejected.

## Attempt to disprove the decision

The processor cannot enforce evaluator purity. An evaluator implementation could
perform I/O or mutate external state. Composition review and tests remain
necessary.

Strict version gating may discard a valid observation after an unrelated state
change. This conservative policy prevents accidental cross-snapshot reasoning.
A future dependency-scoped validation system may permit narrower rebasing, but
no hidden rebase occurs here.

Sensory channels are stable IDs rather than a fixed enum. This supports custom
senses and devices but requires modules to define canonical channel IDs.

The model returns transient observations. Durable observation inboxes, working
memory, knowledge, and long-term memory remain future layers.

## Consequences

Positive:

- explicit separation of objective state and subjective evidence;
- observer-specific and channel-specific provenance;
- stale inputs rejected before evaluation;
- mid-evaluation world changes discard output without retry;
- exact typed evaluator routing;
- no World State mutation;
- no hidden memory, knowledge, belief, event, persistence, or provider behavior;
- spatial and presentation policies remain replaceable.

Negative:

- hosts must assign stimulus and channel IDs;
- observations are not retained automatically;
- strict version gating can discard still-useful evidence;
- evaluator purity depends on implementation discipline;
- visibility, hearing, and other sensory policies are still separate work.

## Enforcement

Tests verify confidence initialization, envelope metadata and validation,
concrete exact types, duplicate and single-use registration, world/version/tick
preflight rejection, provenance-preserving observation, explicit ignore,
exception rollback, independent exact type pairs, mid-evaluation conflicts
without retry, spatial same-place/radius evaluation, and explicit ignoring
across places without inventing pathfinding or occlusion.

Repository verification enforces immutable envelopes and observations, exact
typed evaluators, non-zero observed confidence, semantic separation from facts,
knowledge, belief, and memory, pre- and post-evaluation snapshot gating, and
absence of World State mutation, event dispatch, queues, retries, clocks,
generated IDs, persistence, I/O, providers, presentation geometry, and
game-domain vocabulary.
