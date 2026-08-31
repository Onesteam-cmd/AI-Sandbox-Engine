# ADR-0011: Version-gated exact-type command processing

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Command semantics, stale-context rejection, exact-type handlers,
  authoritative commits, and concurrency conflicts

## Context

The first foundation distinguishes immutable World State, completed-fact events,
deterministic simulation, and snapshot persistence. It still lacks a formal
boundary for requests to change the world.

Player input, network messages, scripted actions, validated LLM output, and
future automation all begin as intentions. They may be stale, impossible, or
invalid. Treating them as events would incorrectly claim they already happened.
Applying them directly would bypass World State authority.

The command layer must reject stale AI and external decisions before expensive
handler evaluation whenever possible and must remain free of queues, provider
calls, event dispatch, persistence, and game-specific vocabulary.

## Decision

Introduce:

- `IEngineCommand` — immutable request marker;
- `CommandIdKind` — externally assigned typed command identity;
- `CommandEnvelope<TCommand>` — command payload plus target world, expected
  World State version, and expected logical tick;
- `CommandContext<TState,TCommand>` — exact envelope and observed snapshot;
- `ICommandHandler<TState,TCommand>` — pure exact-type validation contract;
- `CommandDecision<TState>` — accepted next state or rejected reason;
- `CommandProcessorBuilder<TState>` — single-use exact handler registration;
- `CommandProcessor<TState>` — version-gated command execution;
- `CommandExecutionResult<TState>` — explicit application and failure outcome.

One exact concrete command type has at most one registered handler in one
processor. Concrete value types and sealed reference types are supported.
Interfaces, abstract command types, open generic types, and unsealed reference
types are rejected.

## Command versus event

A command means:

> Attempt this change if it is still valid.

An event means:

> This fact has already occurred.

Commands can be rejected. Events cannot be retroactively rejected by handlers.
The command processor does not dispatch events. A later runtime orchestration
layer may derive completed-fact events only after a successful command commit.

## Stale-context gate

Every command envelope records:

- intended world ID;
- expected World State version;
- expected logical simulation tick.

The processor reads current authority and validates those fields before looking
up or executing the handler.

This is especially important for AI:

1. capture a subjective context from version `V` and tick `T`;
2. call the model outside authoritative execution;
3. parse and validate its structured command;
4. submit an envelope targeting `V` and `T`;
5. reject it before handler execution if the world has advanced.

A stale model response therefore cannot silently act on a different world.

World State Manager still performs its own expected-version checks before and
after handler evaluation. If another writer commits while the pure handler is
running, the command returns `VersionConflict`. The handler is not retried.

## Handler semantics

Handlers are synchronous and side-effect-free. A handler receives one immutable
snapshot and one exact command envelope and returns:

- **Accepted** — a different immutable state reference;
- **Rejected** — a non-empty internal reason.

An accepted handler returning the same state reference is treated as a contract
failure. This prevents successful no-op commands from incrementing World State
version.

A rejection produces no World State commit. A handler exception or malformed
decision escapes as a programming error and leaves authority unchanged.

## Time and version semantics

Commands may increment World State version but do not advance logical simulation
tick. They are state changes at the currently committed tick.

Simulation Scheduler remains the only foundation service that advances logical
tick. For example:

- tick commit: version 1, tick 1;
- accepted command: version 2, tick 1;
- next tick commit: version 3, tick 2.

This keeps logical time separate from the number of authoritative state changes.

## Ordering and concurrency

The processor is caller-driven and contains no queue or command ordering policy.
A future ingress layer will define deterministic ordering across multiple
sources.

Concurrent calls are allowed to race through World State authority. At most one
command based on one expected version can commit. Others receive explicit
version conflicts. The processor never retries a handler.

Command IDs provide stable diagnostics and future deduplication identity, but
Commit 0011 does not persist a processed-command ledger or enforce global
sequence order.

## Alternatives considered

### Apply structured LLM output directly

Would allow stale or invalid model decisions to bypass authoritative validation.
Rejected.

### Represent commands as events

Conflates intentions with completed facts and makes rejection semantics
impossible. Rejected.

### Let handlers mutate World State Manager

Would bypass transition atomicity and make rollback impossible. Rejected.

### Queue commands inside Core

Requires ownership, persistence, ordering, backpressure, restart recovery, and
multi-source policy. Premature for the pure command boundary. Rejected.

### Retry on version conflict

Would execute handlers more than once and could hide stale decisions. Rejected.

### One handler discovered by reflection

Less explicit and vulnerable to assembly or naming changes. Rejected. Handler
registration is deliberate and testable.

### Advance simulation tick for every command

Would make logical world time depend on input volume and network traffic.
Rejected.

## Attempt to disprove the decision

Version and tick gating is strict. A harmless command created one version ago is
still rejected even when its preconditions remain true. This conservatism is
intentional for the foundation. Future command types may support explicit
rebase policies above Core, but no hidden rebase occurs here.

The processor does not prevent two different command processors from registering
different handlers for the same type against one world. Runtime composition must
own one configured processor per world.

Command IDs are not yet used for deduplication. Network retries could submit the
same ID twice if each envelope targets the latest version. A later ingress ledger
can add idempotency without changing command decision semantics.

Reference inequality cannot prove semantic state change. Handlers must still
avoid creating equal replacement objects solely to force version increments.

## Consequences

Positive:

- intentions are formally distinct from completed facts;
- stale player, network, and LLM actions are rejected before handler execution;
- every accepted command commits only through World State Manager;
- command handlers execute at most once per processor call;
- version conflicts are explicit and unretried;
- command state changes preserve logical simulation tick;
- exact command types cannot collide;
- no provider, event, queue, persistence, or game coupling enters Core.

Negative:

- no multi-source ordering or command queue exists;
- strict version gating can reject still-valid intentions;
- command IDs are not yet deduplicated;
- accepted semantic no-ops cannot be detected beyond state-reference equality;
- completed-fact event derivation remains a separate orchestration concern.

## Enforcement

Tests verify envelope metadata, empty and null rejection, exact concrete type
policy, duplicate registration, single-use builders, missing handlers,
world/version/tick preflight rejection, accepted and rejected commands,
tick preservation, exception rollback, malformed handler decisions, exact-type
routing, mid-evaluation version conflicts without retry, and composition with
Simulation Scheduler.

Repository verification enforces command/event semantic separation, immutable
envelopes, explicit handler accessibility, pre-handler stale gates, exact-type
routing, World State-only commits, no-op reference rejection, and absence of
queues, timers, hidden threads, retries, event dispatch, persistence, clocks,
ID generation, I/O, provider calls, and game-domain vocabulary.
