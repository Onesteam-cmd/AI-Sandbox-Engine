# ADR-0043: Host Recovery Checkpoint and Continuation Contracts

- Status: Accepted
- Date: 2026-08-04
- Commit: 0043

## Context

Commit 0042 established bounded immutable active-work snapshots and deterministic
reconciliation between sequential external observations. Core already provides
transport-independent checksum-protected World Snapshot documents and explicit
codec restore results, but it lacks one Host-level authority that binds the
runtime lifecycle, validated composition, queue, active work, and World State
snapshot needed for recovery.

Generic Core must define that authority without taking ownership of storage,
serialization, process discovery, runtime restart, scheduling, or execution.

## Decision

Core introduces:

- `HostRuntimeRecoveryCheckpoint<TRequest>` as an externally identified,
  revisioned immutable aggregate of lifecycle, composition, queue, active-work,
  and checksum-protected World Snapshot authority;
- `HostRuntimeRecoveryContinuation<TRequest, TState>` as immutable authority
  that binds one checkpoint to one successfully restored World State snapshot;
- `HostRuntimeRecoveryCheckpointResult<TRequest>` and
  `HostRuntimeRecoveryContinuationResult<TRequest, TState>` as explicit results;
- `HostRuntimeRecoveryStatus` as explicit checkpoint and continuation outcomes;
- `HostRuntimeRecoveryFlow.CaptureCheckpoint` and
  `HostRuntimeRecoveryFlow.Continue` as pure validation decisions.

Checkpoint capture requires exact runtime identity between lifecycle and
active-work authority, exact composition identity, a checkpoint tick that does
not precede active-work observation, the currently supported World Snapshot
format, and a valid payload checksum.

Continuation requires the optimistic checkpoint revision, an already completed
successful persistence restore result, exact World ID, World State version, and
logical simulation tick from checkpoint authority, and a continuation tick that
does not precede checkpoint capture. Successful continuation advances the
checkpoint revision by exactly one.

## Boundaries

This increment does not:

- read or write files, databases, streams, or transport payloads;
- encode, decode, serialize, migrate, or store checkpoint data;
- enumerate processes, workers, queues, or active work;
- start, stop, restart, schedule, dispatch, or execute runtime work;
- infer terminal outcomes from absent active-work observations;
- read wall-clock time or create background services, locks, timers, or threads.

Persistence codecs remain responsible for producing
`SnapshotRestoreResult<TState>`. Host and infrastructure adapters remain
responsible for storage, process supervision, restart policy, and execution.

## Consequences

Host adapters can persist or transport their chosen representation externally,
restore World State through the existing persistence boundary, and then ask Core
for deterministic continuation authority. Invalid lineage, revisions, format,
checksum, restore, world identity, state version, and logical-tick relationships
remain explicit without hidden side effects.
