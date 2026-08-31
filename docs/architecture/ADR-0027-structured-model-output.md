# ADR-0027: Structured Model Output Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0027 Structured Model Output Contracts`

## Context

Provider-neutral model invocation returns one exact `IModelResponse`, but that
response is still adapter output rather than a validated semantic contract for
conversation, social coordination, behavior, or presentation. The Core needs a
stable boundary that converts a completed response into one exact
host-defined structured payload while preserving source and authority
correlation.

The boundary must not make model output authoritative. It must not assume JSON,
a concrete provider SDK, a closed list of directives, or automatic execution.
It must also reject stale model responses before spending work on decoding and
discard a decoder decision if authority changes during evaluation.

## Alternatives considered

### Put one universal response DTO in Core

Rejected. A fixed DTO containing dialogue, emotions, actions, memories, and
relationship changes would freeze game-specific semantics into the generic
engine and become increasingly sparse and incompatible as products diverge.

### Parse JSON directly inside Core

Rejected. JSON is one transport representation, not the semantic contract.
Owning a serializer, schema language, provider repair loop, or tolerant parser
inside Core would couple the foundation to adapter behavior and hidden policy.

### Let model adapters return authoritative commands directly

Rejected. Adapter output is untrusted external output. Even a successfully
decoded action proposal must pass the existing Behavior and Action Validation
boundary before any command can be constructed or executed.

### Decode without checking current authority

Rejected. Model calls are asynchronous and can complete after the represented
world version or tick has advanced. Structured output must preserve and verify
the original subjective scope.

## Decision

Commit 0027 introduces:

- exact immutable `IStructuredModelOutput` payloads;
- externally assigned output, decoder, and opaque schema IDs;
- positive explicit schema versions;
- stable machine-readable decoder rejection codes;
- an immutable decoding request containing one completed provider-neutral model
  response;
- a synchronous pure exact decoder boundary;
- explicit decoded or rejected decoder decisions;
- one processor bound to one World State manager, decoder, schema ID, and schema
  version;
- preflight validation of decoder, schema, world, version, and tick;
- one decoder invocation followed by one authority re-read;
- explicit discard when authority changes;
- a validated structured-output envelope carrying complete source invocation,
  adapter, profile, prompt, owner, world, version, and tick correlation.

The output payload type defines its own semantic schema outside generic Core.
A detective game, Living World game, or future tool can use different exact
payloads without adding a closed directive enum to the engine.

## Invariants

1. Model-response and structured-output payloads are exact value types or
   sealed reference types.
2. All IDs are externally assigned and non-empty.
3. Schema versions are initialized and positive.
4. One processor is bound to one exact response/output pair and one schema.
5. Decoder, schema ID, and schema version must match the request.
6. The source response must match current world, version, and tick.
7. One processor invokes its decoder at most once.
8. Decoder output is discarded if authority changes during decoding.
9. A decoded envelope copies correlation from the completed model response.
10. Rejection is explicit and carries a stable machine-readable code.
11. Core does not parse JSON or own provider repair policy.
12. Decoding does not execute actions, record turns, synthesize speech, update
    subjective stores, dispatch events, or mutate World State.

## Consequences

Positive:

- model output becomes a typed, testable, provider-neutral semantic artifact;
- every payload retains complete provenance back to the prompt and model call;
- different games can evolve independent structured schemas;
- stale asynchronous output is rejected deterministically;
- downstream address, social, behavior, and presentation orchestration receives
  exact data instead of raw provider text;
- the first Unreal bridge can exchange explicit host DTOs without making Unreal
  or a provider part of Core.

Negative:

- host adapters must implement concrete serialization and schema validation;
- invalid provider formatting must be rejected or repaired outside Core;
- each schema evolution needs a new explicit schema version and compatible
  decoder;
- decoding alone does not perform any visible game action.

## Deferred

Commit 0027 does not implement:

- JSON, protobuf, tool-calling, grammar, or provider-specific parsing;
- model retries, repair prompts, fallback models, or timeouts;
- a universal dialogue/action/emotion DTO;
- automatic Address Resolution or Social Turn-Taking invocation;
- automatic Behavior Intent, Action Proposal, Command, Knowledge, Memory, or
  Relationship updates;
- speech synthesis, subtitles, facial animation, or Unreal presentation;
- host process transport or Unreal bridge integration.

## Enforcement

Tests cover exact payload policy, positive schema versions, stable rejection
codes, request/source correlation, one decoder call, complete output
correlation, explicit rejection, all preflight mismatches, authority changes,
exception propagation, schema freedom, and read-only non-authoritative
behavior.

Repository verification enforces exact request/output/decoder contracts, one
decoder call, two authority reads, schema and source correlation, explicit
statuses, positive versions, and absence of provider, serialization, transport,
retry, adjacent orchestration, state mutation, game-specific, and public-setter
concerns.
