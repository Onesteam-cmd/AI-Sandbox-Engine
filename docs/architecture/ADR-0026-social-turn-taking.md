# ADR-0026: Social Turn-Taking Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0026 Social Turn-Taking Contracts`

## Context

Commit 0025 established a stable conversation roster, completed turns, and
semantic response audiences. The Core still needs an explicit boundary for
deciding whether any participant receives the next speaking turn and whether a
candidate is requesting an ordinary response or an interruption.

This layer must coordinate the speaking floor without becoming a hidden queue,
without calling a model, and without recording the next conversation turn.
Nonverbal reactions, silent observation, memory encoding, and physical behavior
remain independent explicit systems.

## Alternatives considered

### Give every addressed participant an automatic response turn

Rejected. Group conversations would become deterministic cascades and several
NPCs could answer simultaneously. Addressing identifies possible relevance; it
does not grant the speaking floor.

### Maintain an internal mutable speaking queue in Core

Rejected. A hidden queue would create timing, cancellation, persistence, and
authority semantics outside World State. Commit 0026 therefore coordinates one
bounded set of proposals and returns one explicit result.

### Hard-code one deterministic priority formula

Rejected as the only policy. Concrete games may consider direct addressing,
social role, interruption urgency, relationship state, recent participation,
and model-derived structured cues. Core validates a host-defined priority and
an exact coordinator decision while preserving deterministic input ordering.

### Let the coordinator execute the next response

Rejected. Granting the floor is not speech generation, behavior execution, or a
completed conversation turn. Those remain later explicit orchestration steps.

## Decision

Commit 0026 introduces:

- exact immutable `ISocialTurnProposal` payloads;
- externally assigned coordination, coordinator, and proposal IDs;
- explicit `Response` and `Interruption` request kinds;
- initialized host-defined priorities in basis points;
- stable machine-readable no-turn and rejection codes;
- bounded coordination requests containing zero through 63 proposals;
- one proposal per participant and deterministic ordering by descending
  priority, then stable proposal ID;
- correlation with the current conversation, speaker, audience, world,
  authority version, simulation tick, and conversation revision;
- one exact `ISocialTurnCoordinator<TWorldState,TProposal,TTopic>` call;
- explicit grant, no-turn, rejection, empty-proposal, stale, mismatched,
  invalid-proposal, authority-changed, and invalid-selection outcomes;
- a selected proposal only after exact validation against the candidate set.

## Invariants

1. Proposal payloads are exact value types or sealed reference types.
2. IDs are externally assigned and non-empty.
3. A request contains at most 63 proposals.
4. Proposal IDs and participant IDs are unique within one request.
5. Proposal ordering is deterministic.
6. The request must match the current completed conversation turn.
7. The current speaker cannot propose the next turn.
8. Every proposing participant belongs to the conversation.
9. One processor invokes one configured coordinator at most once.
10. Empty proposal sets skip the coordinator explicitly.
11. Coordinator output is discarded if authority changes.
12. A grant must reference exactly one supplied proposal.
13. Coordination never records a turn or mutates World State.
14. No model, speech, prompt, retrieval, address, command, event, or timer
    orchestration is hidden in the social module.

## Consequences

Positive:

- group conversations can arbitrate one next speaker without simultaneous
  responses;
- direct responses and interruptions share one validated boundary;
- silence is explicit rather than represented by a failed model call;
- concrete games retain freedom to define social policy;
- future Unreal integration receives one stable selected participant and
  proposal rather than an implicit queue.

Negative:

- the host must gather participant proposals before coordination;
- Core does not decide how relationships, gaze, volume, or semantic content
  affect priority;
- the selected participant still requires structured model output and behavior
  orchestration;
- nonverbal reactions and memory-only processing are not coordinated here.

## Deferred

Commit 0026 does not implement:

- model-generated structured response schemas;
- speech generation, audio playback, subtitles, or lip sync;
- automatic response text or behavior;
- mutable speaking queues, timers, delays, or turn reservations;
- participant join/leave;
- nonverbal reaction arbitration;
- Knowledge, Memory, Relationship, Event, Command, or World State mutation;
- Unreal integration.

## Enforcement

Tests cover exact proposal policy, bounded values, deterministic ordering,
one proposal per participant, empty proposal sets, response and interruption
grants, explicit no-turn and rejection, preflight correlation, invalid
participants and selections, authority changes, and exception propagation.

Repository verification enforces one coordinator call, two authority reads,
current-turn and audience correlation, deterministic ordering, explicit status
semantics, and absence of provider, transport, retry, scheduling, command,
event, state-mutation, game-specific, and public-setter concerns.
