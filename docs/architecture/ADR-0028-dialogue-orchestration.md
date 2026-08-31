# ADR-0028: Dialogue Orchestration Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0028 Dialogue Orchestration Contracts`

## Context

The Core already exposes separate boundaries for speech, conversation state,
address resolution, context retrieval, prompt composition, model invocation,
structured output, social turn-taking, and action validation. A host still needs
one stable way to decide what should happen next in a dialogue exchange without
hiding those boundaries inside a monolithic pipeline.

The orchestration boundary must preserve subjective owner, source speaker,
conversation, audience, world, version, tick, and prior-artifact provenance. It
must remain read-only and must not call providers, execute returned directives,
or mutate authoritative or subjective state.

## Alternatives considered

### Build one automatic end-to-end dialogue pipeline inside Core

Rejected. It would hide latency, retry, provider selection, transport, and
cross-system mutation policy. It would also make partial failure and Unreal host
integration difficult to observe and test.

### Encode a closed enum of every next operation

Rejected. Concrete products will need different provider calls, presentation
steps, moderation, caching, and gameplay branches. Exact host-defined directive
payloads preserve extensibility without weakening correlation.

### Let structured model output execute actions directly

Rejected. Structured output remains untrusted. Action proposals still require
the existing action-validation boundary and commands still require authoritative
execution through the runtime.

### Store a mutable hidden exchange queue

Rejected. Hidden queues obscure ordering and recovery. The host explicitly
supplies prior immutable artifacts and receives one directive, completion, or
rejection per call.

## Decision

Commit 0028 introduces:

- exact immutable dialogue input, artifact, directive, and completion payloads;
- externally assigned exchange, orchestration, orchestrator, artifact, and
  artifact-source IDs;
- positive host-assigned artifact sequences;
- stable machine-readable rejection codes;
- immutable artifact envelopes with complete exchange and authority correlation;
- immutable bounded requests containing zero through 128 deterministic artifacts;
- one synchronous pure exact orchestrator boundary;
- explicit continue, complete, and reject decisions;
- one processor bound to current authority and one exact orchestrator;
- preflight world, version, tick, conversation, revision, participant, audience,
  and artifact validation;
- one orchestrator call followed by one authority re-read;
- explicit discard when authority changes.

A directive is only a host instruction. The host may use it to invoke one of the
existing processors, wait for external work, request presentation, or perform a
product-specific step. Core does not interpret or execute it.

## Invariants

1. Input, artifact, directive, completion, and topic payloads are exact value
   types or sealed reference types.
2. All IDs are externally assigned and non-empty.
3. Artifact sequences are positive and unique within a request.
4. Artifacts are deterministically ordered by sequence and stable artifact ID.
5. Every artifact matches exchange, conversation, perspective owner, world,
   version, and tick correlation.
6. The perspective owner and source speaker are current conversation participants.
7. The resolved audience is valid for the current roster.
8. One processor invokes one orchestrator at most once.
9. Authority is read before and after orchestration; changes discard the decision.
10. Continue, complete, and rejection are mutually exclusive and explicit.
11. Returned directives are never executed by the dialogue module.
12. No adjacent processor, provider, event dispatcher, subjective store, or World
    State mutation is hidden inside the module.

## Consequences

Positive:

- the `.NET Host` receives one stable coordination point for the full dialogue
  workflow;
- Unreal can observe every external step and correlate it to one exchange;
- different games can define exact directives and completions independently;
- partial progress can be persisted outside Core as explicit artifacts;
- stale asynchronous results are rejected deterministically;
- action validation and authoritative command execution remain separate.

Negative:

- the host must implement the concrete orchestration policy and dispatch loop;
- artifacts add explicit plumbing and correlation IDs;
- provider retry, timeout, cancellation, and persistence policy remain host work;
- a completed Core decision alone does not produce visible dialogue or gameplay.

## Deferred

Commit 0028 does not implement:

- a concrete dialogue host loop or dependency injection container;
- provider selection, retries, fallback, timeout, caching, or transport;
- JSON DTOs or a fixed list of dialogue operations;
- automatic speech recognition, retrieval, prompt, model, social, or action calls;
- command execution, turn recording, Knowledge, Memory, or Relationship updates;
- subtitles, audio playback, facial animation, lip sync, or Unreal integration;
- durable exchange persistence or recovery.

## Enforcement

Tests cover exact payload policy, bounded codes and sequences, deterministic
artifact ordering, full request correlation, continue/complete/reject decisions,
all preflight gates, closed conversations, invalid audiences, authority changes,
exception propagation, and read-only non-execution.

Repository verification enforces the request, artifact, orchestrator, and
processor contracts; one orchestrator call; two authority reads; explicit
statuses; full correlation; and absence of providers, transport, serialization,
retry, adjacent processor calls, authority mutation, game vocabulary, and public
setters.
