# ADR-0004: Deterministic in-process event foundation

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Core event contracts and dispatch semantics

## Context

The engine must be event-driven, but the event foundation cannot become a
second source of truth, a hidden task scheduler, or an implicit persistence
mechanism. World State remains authoritative. Future entity, component,
simulation, AI, gameplay, and presentation layers need a common way to describe
completed facts and react to them without direct coupling.

The foundation also has to support deterministic tests, replays, and
authoritative simulation. Wall-clock timestamps, internally generated IDs,
parallel handler execution, and unspecified ordering would weaken that goal.

## Decision

Introduce the following dependency-free core types:

- `IEngineEvent` — marker for immutable fact notifications;
- `EventIdKind` — typed-ID marker for event occurrences;
- `EventEnvelope<TEvent>` — validated payload plus event ID, total-order
  sequence, and simulation tick;
- `IEventHandler<TEvent>` — invariant exact-type asynchronous handler contract
  with explicit public interface members;
- `EventDispatcherBuilder` — single-use registration phase;
- `EventDispatcher` — immutable registration table and sequential dispatcher.

The authoritative runtime supplies:

- the event identifier;
- the total-order sequence;
- the simulation tick;
- the immutable payload.

The handler contract is invariant because `TEvent` appears inside the invariant
`EventEnvelope<TEvent>` parameter. The dispatcher performs exact generic
payload-type lookup. Matching handlers are awaited one at a time in registration
order. Cancellation is checked before and
between handlers. A handler failure stops the current chain and propagates to
the caller.

The dispatcher does not:

- modify World State;
- generate IDs;
- read wall-clock time;
- enqueue or retain events;
- persist events;
- retry failures;
- create threads or parallel tasks;
- discover handlers through reflection or a service locator.

Separate dispatch calls are not globally serialized. The future authoritative
runtime and simulation scheduler own that ordering boundary.

## Terminology

An event states that something has already happened. A request to make
something happen is a command or intent and is not modeled as an engine event.

The event sequence is an authoritative total-order number assigned by the
runtime. The simulation tick is logical simulation time. Neither value implies
wall-clock time.

## Alternatives considered

### Framework or mediator package

A package could provide registration and dispatch quickly, but it would make a
third-party API foundational, often add reflection or dependency injection, and
obscure ordering semantics. Rejected for the core boundary.

### Global static event bus

Convenient but creates hidden process-wide state, weak test isolation, and
unclear ownership. Rejected.

### Parallel handler invocation

May reduce latency for independent handlers, but creates nondeterministic
completion order and complicated failure semantics. Rejected at the
authoritative foundation. Explicit non-authoritative projections may parallelize
later outside this contract.

### Persistent event sourcing

An append-only event log may eventually be useful, but selecting event sourcing
before World State and persistence requirements exist would conflate
notification, authority, and storage. Deferred. World State remains the source
of truth.

### Synchronous-only handlers

Simple and allocation-efficient, but future adapters may need asynchronous I/O.
Rejected. `ValueTask` keeps synchronous completion inexpensive while retaining
an async boundary.

### Base-type and interface fan-out

Automatically invoking handlers registered for base interfaces can be useful,
but it introduces non-obvious handler sets and ordering across inheritance.
Rejected. Dispatch is exact-type only.

## Attempt to disprove the decision

Sequential dispatch can allow a slow handler to delay later handlers.
Nevertheless, the authoritative path benefits from explicit backpressure and
failure propagation. Slow external work should be separated into validated
non-authoritative adapters rather than hidden inside the core dispatcher.

The generic dictionary stores handler lists behind `object`. This is an internal
implementation detail guarded by generic registration and exact-type lookup.
It avoids reflection and per-dispatch handler discovery. If profiling later
shows that this structure is inadequate, the implementation can change without
altering the public contracts.

The decision would need revision if the simulation requires transactional
multi-event publication, durable subscriptions, cross-process delivery, or
formal event sourcing. None of those requirements is proven at this stage.

## Consequences

Positive:

- explicit deterministic handler order;
- no hidden concurrency or global state;
- typed event IDs and payload contracts;
- logical simulation time rather than wall-clock coupling;
- no dependency on gameplay, DI, reflection, or external packages;
- clear failure and cancellation behavior;
- future World State can publish validated facts without surrendering authority.

Negative:

- slow handlers block later handlers in the same dispatch;
- handler immutability and idempotency remain implementation responsibilities;
- separate dispatch calls can overlap unless the runtime serializes them;
- no persistence, replay store, wildcard subscriptions, or automatic retries;
- event ID and ordering assignment must be supplied by later runtime services.

## Enforcement

Tests verify envelope validation, struct and reference payloads, exact-type
routing, deterministic registration order, cancellation, failure propagation,
single-use builder behavior, and the approved public surface. Repository
verification rejects wall-clock access, hidden ID generation, parallel dispatch,
queues, retries, and persistence behavior inside the foundation dispatcher.
