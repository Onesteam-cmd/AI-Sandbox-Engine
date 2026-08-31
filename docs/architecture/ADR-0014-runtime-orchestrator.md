# ADR-0014: Caller-driven non-queuing runtime orchestrator

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Shared authority composition, operation admission, caller order,
  completed commit facts, concurrency, and post-commit boundaries

## Context

Commands and Simulation Scheduler already share World State semantics, but a
host could accidentally construct them against different managers, race them
without a clear admission rule, or hide event dispatch and persistence inside
ad hoc glue.

The engine now needs one reusable runtime boundary that composes the existing
subsystems without becoming a game loop, queue, timer, service host, event bus,
or persistence coordinator.

The caller must retain authority over operation order. A desktop game, server,
test runner, and Unreal adapter may pace execution differently while using the
same deterministic core.

## Decision

Introduce:

- `RuntimeOrchestratorBuilder<TState>` — registers exact command handlers and
  ordered simulation systems, then builds both against one World State Manager;
- `RuntimeOrchestrator<TState>` — admits one command or one tick at a time;
- `RuntimeInvocationStatus` — admitted or busy;
- `RuntimeCommandResult<TState>` and `RuntimeTickResult<TState>` — wrapper
  results preserving the normal subsystem outcome;
- `RuntimeCommitFact` — completed metadata for one successful authoritative
  commit;
- `RuntimeCommitKind` — command or simulation tick.

The orchestrator adds no alternative mutation path. Commands still commit only
through Command Processor and ticks still commit only through Simulation
Scheduler.

## Admission and ordering

The orchestrator does not queue and does not wait.

It uses one atomic admission flag:

```text
idle
  ↓ admit one call
command or tick executes
  ↓
admission released
```

If another call arrives while an operation is in progress, it returns `Busy`
immediately. It does not block, retry, reorder, or execute later.

Therefore deterministic order remains explicit caller order. A host that accepts
input from multiple threads must establish its own deterministic ingress order
before calling Core.

The same rule prevents reentrant execution from handlers or systems: a nested
runtime call receives `Busy` rather than deadlocking or creating a hidden second
commit.

## Completed commit fact

A successful operation returns `RuntimeCommitFact`, which implements
`IEngineEvent` and contains:

- operation kind;
- world ID;
- previous and current World State versions;
- previous and current simulation ticks;
- command ID for command commits.

Command facts preserve the tick. Simulation facts advance it by exactly one.

Rejected commands, rejected ticks, missing handlers, conflicts, and busy calls
produce no commit fact because no authoritative commit occurred.

## Explicit post-commit work

The orchestrator never:

- creates an event ID;
- chooses an event sequence;
- creates an `EventEnvelope`;
- dispatches an event;
- persists a snapshot;
- uploads data;
- retries a failure.

After a successful operation, the host may:

1. wrap the returned fact in an externally identified event envelope;
2. dispatch it through Event Dispatcher;
3. persist the returned authoritative snapshot;
4. update presentation or networking.

Failures in those post-commit activities cannot roll back the already committed
World State. Their recovery policy belongs to the host or infrastructure layer.

## External World State writers

The builder ensures the internal Command Processor and Scheduler share one
manager. It cannot prevent external code holding the manager from attempting a
transition directly.

Existing expected-version checks remain authoritative. If an external writer
commits while a runtime command is evaluating, the command returns
`VersionConflict` and no commit fact. The handler is not retried.

Production composition should expose the orchestrator rather than the underlying
manager wherever possible.

## Read behavior

`Read()` is always available and does not acquire runtime operation admission.
World State snapshots are immutable, and World State Manager already provides an
atomic current snapshot reference.

A read during an operation sees either the previous committed snapshot or a
later committed snapshot, never a partial transition.

## Alternatives considered

### Put a FIFO command queue inside Core

Requires persistent ordering, backpressure, cancellation, shutdown, retry, and
multi-source policy. It would hide caller order and broaden the foundation
prematurely. Rejected.

### Block concurrent calls with a lock

Serializes work but lets thread scheduling decide hidden order and can deadlock
under reentrancy. Rejected.

### Dispatch events automatically after commit

Requires event IDs, sequences, failure policy, and async orchestration. It also
makes committed state appear to fail when a handler fails afterward. Rejected.

### Persist automatically after every commit

Chooses storage cadence and failure semantics that differ between games,
servers, and tests. Rejected.

### Merge Command Processor and Scheduler implementations

Would duplicate and weaken already tested subsystem contracts. Rejected. The
orchestrator delegates to them.

### Return only the final snapshot

Loses the explicit completed operation metadata needed for event dispatch,
diagnostics, audit, and host reactions. Rejected.

## Attempt to disprove the decision

A `Busy` result means the caller must retry or reject input explicitly. This is
deliberate, but a high-throughput server will need an ingress queue above Core.

Atomic admission does not guarantee fairness. No waiting occurs, so fairness is
the caller's scheduling responsibility.

The commit fact derives the previous version and tick from frozen one-step
semantics. If a future operation can advance by more than one version or tick,
it will need a different fact factory and ADR update.

Post-commit event dispatch is not atomic with the state commit. A process crash
between commit and durable event publication can lose a notification. A future
infrastructure outbox can solve this using persisted commit facts without
moving storage into Core.

## Consequences

Positive:

- Command Processor and Scheduler share one guaranteed authority;
- caller order remains explicit;
- no hidden queue, wait, retry, or thread scheduling;
- reentrant and concurrent operations fail fast;
- only successful commits produce completed facts;
- event dispatch and persistence remain explicit post-commit work;
- existing command, tick, conflict, and rollback semantics are preserved;
- games, servers, tests, and engine adapters share one composition boundary.

Negative:

- callers must handle `Busy`;
- no built-in fairness or queue exists;
- event publication is not transactionally durable with World State;
- external holders of World State Manager can still create conflicts;
- operation pacing remains a host responsibility.

## Enforcement

Tests verify single-use construction, command commit facts, tick commit facts,
no hidden dispatch, no facts for rejected operations, immediate busy behavior,
external conflicts without retry, explicit caller order, and byte-identical
save/restore continuation across mixed command and tick operations.

Repository verification enforces shared manager construction, immutable commit
facts, atomic non-waiting admission, delegation to existing subsystems, fact
creation only after successful commits, and absence of event dispatch, queues,
locks, waits, retries, persistence, clocks, generated IDs, providers, I/O,
hidden execution, and game-domain vocabulary.
