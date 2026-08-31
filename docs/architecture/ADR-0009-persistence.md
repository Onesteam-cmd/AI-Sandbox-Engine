# ADR-0009: Versioned transport-independent World Snapshot persistence

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Snapshot envelope, stable schema identity, integrity validation,
  codec boundaries, and authoritative restoration

## Context

World State, entities, components, and deterministic scheduling now produce
complete immutable snapshots. The engine needs a persistence boundary before
concrete storage adapters exist.

Core must not choose a file path, JSON layout, database, compression algorithm,
cloud provider, or game-specific component schema. At the same time, save data
must preserve world identity, authoritative state version, logical simulation
tick, stable schema identity, and corruption detection.

CLR type and assembly names are unsuitable as persistent identity because
renaming code would invalidate saves even when the data schema remains
compatible.

## Decision

Introduce a transport-independent snapshot envelope:

- `SnapshotFormatVersion` — outer envelope contract version;
- `PersistenceSchemaId` — stable lowercase dot-separated schema identity;
- `PersistenceSchemaVersion` — positive payload schema version;
- `SnapshotPayload` — immutable defensive byte ownership;
- `SnapshotChecksum` — canonical SHA-256 payload integrity;
- `WorldSnapshotDocument` — envelope metadata, payload, and checksum;
- `IWorldStateSnapshotCodec<TState>` — deterministic state payload codec;
- `WorldStateDecodeDecision<TState>` — accepted state or explicit rejection;
- `WorldStateSnapshotPersistence<TState>` — capture and restore validation;
- `SnapshotRestoreStatus` and `SnapshotRestoreResult<TState>` — explicit
  compatibility and corruption outcomes.

`WorldStateManager<TState>.Restore(snapshot)` adopts a snapshot only after a
persistence codec has reconstructed and validated it. The manager itself does
not read storage or decode bytes.

## Snapshot capture

Capture performs:

1. accept one immutable authoritative snapshot;
2. encode only its state root through the configured codec;
3. reject a null payload as a codec contract failure;
4. compute SHA-256 over the exact encoded bytes;
5. create an envelope containing:
   - current outer format version;
   - stable schema ID;
   - emitted schema version;
   - world ID;
   - World State version;
   - logical simulation tick;
   - immutable payload;
   - checksum.

Equal state and codec configuration must produce equal payload bytes. The
foundation does not inject timestamps, random IDs, file names, or host metadata.

## Snapshot restore

Restore validates in this order:

1. supported outer envelope format;
2. exact stable schema ID match;
3. codec support for the stored schema version;
4. payload checksum;
5. codec decode decision;
6. decoded decision consistency;
7. immutable snapshot reconstruction with stored metadata.

The codec is never called for an unsupported outer format, schema mismatch,
unsupported schema version, or checksum failure.

Expected incompatibility and corruption are explicit result statuses. A null or
internally inconsistent codec decision is a programming error and throws.

## Stable schema identity

`PersistenceSchemaId` is a semantic schema name such as:

- `game.world`;
- `component.position`;
- `memory.event`.

It is not a CLR namespace, type name, assembly-qualified name, file name, or
database table name.

The current root codec owns one stable schema ID. A future game-data layer can
use the same value type to register stable component and subsystem codec IDs.

## Storage separation

Core returns and accepts `WorldSnapshotDocument`. It performs no:

- file I/O;
- stream I/O;
- database access;
- JSON or MessagePack selection;
- compression;
- encryption;
- cloud upload;
- backup rotation;
- atomic file replacement.

Infrastructure adapters later map document fields to a concrete transport. This
allows local files, SQLite, server storage, tests, and cloud saves to share one
compatibility and integrity policy.

## Component persistence

`ComponentRegistry` remains exact-type in memory. A concrete game codec knows
the approved component schema catalog and can enumerate each registered
component type through typed APIs.

Runtime CLR `Type` keys must never be written as persistent identities. Concrete
component codecs will use stable `PersistenceSchemaId` values and explicit
versions.

## Alternatives considered

### Serialize the entire object graph automatically

Convenient, but binds saves to CLR names, private layouts, constructors, and
serializer behavior. It also makes migrations implicit. Rejected.

### Add JSON directly to Core

Human-readable, but prematurely selects a transport encoding and canonicalization
policy. Rejected. A JSON codec can be implemented outside Core.

### Persist only the payload bytes

Insufficient because world ID, authoritative version, logical tick, schema
identity, compatibility version, and integrity would become adapter-specific.
Rejected.

### Use timestamps for save freshness

Wall-clock time is host metadata rather than authoritative simulation state.
Adapters may store it separately, but Core snapshots do not. Rejected.

### Use event sourcing instead of snapshots

The constitutional authority is World State. An event log may later supplement
replay or audit, but it does not replace complete authoritative snapshots.
Rejected for this foundation.

### Throw for every incompatible or corrupted save

Unsupported versions and checksum mismatches are expected operational outcomes.
Explicit statuses provide cleaner UI and recovery decisions. Rejected.

## Attempt to disprove the decision

A byte payload is intentionally opaque to Core, so Core cannot verify semantic
determinism or deep immutability. Those properties remain codec responsibilities
and require game-data tests.

SHA-256 detects corruption but does not authenticate malicious modification.
Encryption and message authentication require key management and belong in
storage infrastructure. The checksum is integrity metadata, not a security
boundary.

The complete payload is held in memory and copied defensively. Very large worlds
may require streaming or chunked snapshots. Introducing streams now would mix
transport I/O into Core. A future chunk-document abstraction can be added after
real size measurements while preserving schema and envelope semantics.

## Consequences

Positive:

- stable save identity independent of CLR refactoring;
- explicit format and schema evolution;
- preserved world ID, state version, and logical tick;
- corruption detected before decode;
- deterministic codec boundary;
- no storage-vendor or encoding lock-in;
- restored managers continue from the exact saved authority;
- component schema IDs can reuse the same stable identifier primitive.

Negative:

- concrete codecs and storage adapters are still required;
- payloads are memory-resident and defensively copied;
- SHA-256 is not authentication or encryption;
- schema migrations are codec responsibilities;
- Core cannot automatically enumerate arbitrary component schemas.

## Enforcement

Tests verify schema ID canonicalization, positive versions, defensive payload
ownership, known SHA-256 output, checksum mismatch, deterministic capture,
metadata round-trip, restored manager continuation, validation order, unsupported
format and schema outcomes, codec rejection, and null codec contract failures.

Repository verification enforces explicit codec members, stable schema policy,
capture checksum, restore validation order, immutable document fields, defensive
payload copies, SHA-256 integrity, explicit World State restoration, and absence
of file, stream, database, network, clock, ID generation, event dispatch,
simulation execution, hidden threads, queues, and game-domain vocabulary.
