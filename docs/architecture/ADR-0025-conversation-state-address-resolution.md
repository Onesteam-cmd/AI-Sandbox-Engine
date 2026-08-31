# ADR-0025: Conversation State and Address Resolution Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0025 Conversation State and Address Resolution Contracts`

## Context

The Core now has provider-neutral model and speech boundaries, but it still
needs an explicit current conversation model and a semantic address-resolution
boundary. A transcript alone does not identify who should respond. Addressing
may depend on names, semantic content, gaze, recent turns, participant roster,
and the current social situation.

Conversation state must remain distinct from Knowledge, Memory, Relationships,
completed Events, and authoritative World State. Address resolution must be
read-only and must never turn a model suggestion into a command or fact.

## Alternatives considered

### Store conversation state only in prompt history

Rejected. Prompt history is provider-facing derived context, not an
authoritative or reusable current social state. It cannot safely coordinate
turn order, participants, current topic, and resolved audiences.

### Make address resolution a keyword or name-matching utility

Rejected. Name matching can be one signal, but the product requires semantic
and contextual addressing. The generic boundary therefore accepts an exact
structured query and a host-configured resolver rather than hard-coded words.

### Let the resolver call an LLM directly

Rejected inside this Core increment. Model invocation is already an explicit
separate boundary. A host may obtain structured semantic cues from a model and
place them in an exact address query, but the address resolver itself remains
pure, synchronous, deterministic for supplied inputs, and invoked once.

### Store a fixed closed set of conversation topics

Rejected. Concrete games and hosts define exact topic payloads. Generic Core
only enforces exact immutable topic types.

## Decision

Commit 0025 introduces:

- immutable world-scoped `ConversationState<TTopic>`;
- externally assigned stable conversation, resolution, and resolver IDs;
- deterministic participant ordering with a bounded roster;
- positive optimistic conversation revisions and sequential turn numbers;
- explicit current topic, last completed turn, and closed state;
- explicit response audiences: none, specific participants, or all eligible
  participants;
- pure optimistic turn recording, topic revision, and closure results;
- exact immutable semantic address-query envelopes;
- a read-only address context containing the stable authority snapshot,
  conversation state, and eligible participant candidates;
- one exact `IAddressResolver<TWorldState,TQuery,TTopic>` call;
- explicit resolved, rejected, stale, mismatched, closed, authority-changed,
  and invalid-audience outcomes;
- preflight and post-resolution authority checks with no hidden retry.

## Invariants

1. Topic and query payloads are exact value types or sealed reference types.
2. Conversation state is scoped to one world and one stable conversation ID.
3. Participant IDs are non-empty, unique, bounded, and ordinally sorted.
4. Conversation mutations require the expected current revision.
5. Turn numbers are positive and strictly sequential.
6. Speakers and selected targets must belong to the conversation.
7. `AllParticipants` means every participant except the current speaker.
8. One processor invokes one configured resolver at most once.
9. Resolver output is discarded if authority changes during resolution.
10. Address resolution never changes World State or subjective stores.
11. No model, speech, prompt, transport, timer, or provider call is hidden in
    the conversation module.

## Consequences

Positive:

- current conversation state is explicit and independent of prompt history;
- direct, group, broadcast, and no-response addressing share one validated
  representation;
- later social turn-taking can consume a stable roster and resolved audience;
- a host can combine semantic model output with deterministic validation;
- stale authority and stale conversation revisions remain visible.

Negative:

- participant join/leave mutations are deferred;
- the Core does not infer names, gaze, acoustics, or semantic intent itself;
- hosts must define exact topic and address-query payload schemas;
- current conversation state must still be persisted by a concrete game state
  or component integration.

## Deferred

Commit 0025 does not implement:

- social turn-taking, interruption, response arbitration, or speaking queues;
- participant join/leave and nested conversation merging;
- structured model-output schemas;
- concrete name, gaze, spatial, acoustic, or relationship feature extraction;
- automatic transcript-to-query conversion;
- model calls, retries, provider adapters, or prompt composition;
- Knowledge, Memory, Relationship, Command, Event, or World State mutation;
- Unreal integration, presentation, subtitles, lip sync, or audio playback.

## Enforcement

Tests cover exact payload policy, value bounds, deterministic immutable rosters,
optimistic revisions, turn sequencing, topic revision, closure, all audience
kinds, one resolver call, preflight mismatches, explicit rejection, invalid
audiences, mid-resolution authority changes, and exception propagation.

Repository verification enforces the conversation contracts, one resolver call,
two authority reads, explicit status semantics, deterministic ordering, and
absence of model or speech orchestration, command execution, event dispatch,
hidden retry, wall clocks, generated IDs, I/O, concrete providers, game
vocabulary, and public setters.
