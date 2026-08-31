# ADR-0021: Context Retrieval Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0021 Context Retrieval Contracts`

## Context

Knowledge, Memory, Relationships, completed Events, current observations, and
future domain data must be made available to reasoning without dumping the
entire world or granting one NPC another owner's subjective state. Retrieval
must remain deterministic, bounded, inspectable, and separate from prompt
composition and provider calls.

The existing Core already distinguishes authoritative World State, completed
Events, Perception, Knowledge, Memory, Relationships, behavior intent, action
validation, and Commands. Context retrieval must read these layers without
becoming a new source of truth or silently changing them.

## Decision

Introduce an exact typed, owner-scoped context-retrieval contract.

A `ContextQueryEnvelope<TQuery>` carries an externally assigned query ID, one
subjective owner entity, explicit world/version/tick coordinates, a bounded
item limit, and an exact immutable query payload.

A concrete `IContextRetriever<TState,TQuery,TItem>` receives one stable
snapshot, the exact query, and its stable retriever ID. It is invoked at most
once. It returns a `ContextRetrievalDecision<TItem>` containing:

- one or more exact typed context items;
- an explicit successful empty result; or
- an explicit stable machine-readable rejection.

Every returned item carries external identity, the producing retriever ID,
owner, world, relevance, and an exact payload. Decisions sort items by
relevance descending and item ID ordinal ascending. Duplicate item IDs are
invalid.

The processor rejects wrong-world, stale-version, or stale-tick queries before
retrieval. It re-reads authority after retrieval; any version or tick change
discards the decision without retry. Stable retrieved decisions are then
checked for world, owner, retriever, and item-limit consistency.

No closed `ContextSourceKind` enum is introduced. Knowledge, Memory,
Relationships, Events, observations, and future domain sources use separate
exact query/item payloads and retriever implementations. Source-specific
revision and provenance remain in those typed payloads rather than being
flattened into one generic schema.

## Alternatives considered

### Dump complete World State into prompt composition

Rejected. It breaks subjective knowledge boundaries, scales poorly, and makes
prompt content depend on incidental state layout.

### One universal untyped dictionary or JSON context item

Rejected. It removes compile-time contracts, weakens deterministic validation,
and makes schema migration implicit.

### A fixed source enum with mandatory generic axes

Rejected. The Core would prematurely freeze all future context sources and
force unrelated stores into one representation.

### Retrieval that directly updates Memory, Knowledge, or Relationships

Rejected. Reading context is not an authoritative mutation and must not create
cross-layer side effects.

### Automatic retries after authority changes

Rejected. Retrievers may be expensive and non-idempotent outside Core. Hidden
retry would obscure invocation count and snapshot provenance.

## Disproof and failure analysis

Exact generic processors do not yet route heterogeneous queries dynamically.
That is intentional: future composition can explicitly call registered exact
retrievers and merge their validated outputs. A heterogeneous registry may be
added only with exact type keys and deterministic ordering.

Relevance is retriever-assigned and therefore comparable only under that
retriever's documented policy. Prompt budgeting and cross-source normalization
remain deferred.

A valid retrieval result can become stale before later provider inference.
Future prompt and action flows must carry snapshot coordinates and revalidate
before authoritative action execution.

## Consequences

Positive:

- subjective owner scope is explicit;
- retrieval remains read-only and snapshot-bound;
- exact query and item schemas are extensible without untyped blobs;
- results are bounded and deterministically ordered;
- empty and rejected outcomes are distinct;
- wrong-owner and wrong-source output is detected;
- authority conflicts discard results without hidden retry;
- Prompt Composer and providers remain outside the layer.

Negative:

- domains must implement source-specific retrievers and payloads;
- no heterogeneous registry or cross-source ranking exists yet;
- item relevance is not a token budget;
- no semantic embeddings or provider search are included.

## Deferred

Commit 0021 does not implement:

- Prompt Composer or Context Budget Manager;
- token counting, summarization, or prompt formatting;
- embeddings, vector databases, or provider retrieval;
- a heterogeneous retriever registry;
- concrete Knowledge, Memory, Relationship, or Event retrievers;
- conversation history selection;
- asynchronous retrieval, caching, retries, or background work;
- automatic mutations of Knowledge, Memory, Relationships, or World State.

## Enforcement

Tests cover exact payload policy, owner-scoped query coordinates, bounded and
deterministic item ordering, explicit empty/rejected outcomes, stale preflight
gates, returned scope validation, authority conflict after one retriever call,
exception propagation without retry, and source-specific typed payloads.

Repository verification enforces exact generic constraints, one retriever
invocation, pre/post authority reads, bounded output validation, and absence of
World State mutation, subjective-store mutation, providers, clocks, tasks,
I/O, event dispatch, game vocabulary, or a fixed source enum.
