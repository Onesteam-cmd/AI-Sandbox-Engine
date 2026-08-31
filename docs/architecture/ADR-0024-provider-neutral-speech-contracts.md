# ADR-0024: Provider-Neutral Speech Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0024 Provider-Neutral Speech Contracts`

## Context

Provider-neutral model invocation now carries exact request and response payloads
through one explicit asynchronous adapter call. Speech recognition and speech
synthesis require a related but distinct boundary because they operate on audio
or utterance payloads, use recognition or voice profiles, and must remain
separate from recording, playback, lip sync, subtitles, and conversation
coordination.

The Core must preserve subjective owner and authority provenance without
choosing an STT/TTS provider, audio codec, device API, transport, retry policy,
or concrete voice. Adapter output must remain non-authoritative: a transcript
is evidence supplied to later language and conversation systems, while
synthesized audio is presentation output rather than World State.

## Alternatives considered

### Reuse generic model invocation directly

Rejected as the only public boundary. Speech needs an explicit recognition or
synthesis operation and input/output usage limits that remain meaningful to
speech adapters and hosts. Hiding both operations inside generic model payloads
would weaken validation and make device or presentation concerns easier to
leak into generic orchestration.

### Separate duplicated STT and TTS contract families

Rejected for this increment. Both operations have identical correlation,
adapter-decision, cancellation, failure, and usage-validation semantics.
Duplicating complete request, response, decision, result, and processor
families would enlarge public API without increasing authority safety.
`SpeechOperationKind` keeps the operation explicit while exact generic payload
pairs and opaque profiles preserve specialization.

### Put recording and playback in Core

Rejected. Audio capture, codec handling, playback, buffering, streaming, device
selection, and lip synchronization are infrastructure or presentation
responsibilities with platform-specific I/O and timing.

## Decision

Commit 0024 introduces one provider-neutral speech module with:

- exact immutable `ISpeechRequest` and `ISpeechResponse` payload markers;
- an explicit `SpeechOperationKind` containing Recognition and Synthesis;
- externally assigned invocation, adapter, and opaque speech-profile IDs;
- immutable request and response envelopes carrying owner, world, version,
  tick, operation, adapter, profile, and exact payload correlation;
- positive adapter-defined input and output limits;
- initialized non-negative adapter-reported input and output usage;
- stable machine-readable rejection and operational failure codes;
- one exact asynchronous `ISpeechAdapter<TRequest,TResponse>` call;
- explicit completed, rejected, failed, mismatch, and limit-exceeded outcomes;
- cancellation and adapter exceptions that propagate without hidden retry.

The processor does not record audio, play audio, transcode formats, invoke model
or prompt processors, read current authority, execute commands, dispatch events,
or modify any subjective or authoritative store.

## Invariants

1. Every request and response payload type is exact: a value type or sealed
   reference type implementing its marker.
2. Recognition and synthesis are explicit and validated operation values.
3. Invocation, adapter, profile, owner, world, version, and tick are correlated
   across completed responses.
4. Input and output limits are initialized positive values; usage is initialized
   and non-negative.
5. One processor invokes one configured exact adapter at most once.
6. Adapter rejection and operational failure remain distinct.
7. Cancellation and exceptions do not trigger retry.
8. Speech output is never authoritative and cannot mutate state automatically.
9. Device, codec, transport, provider, retry, and presentation policy remain
   outside generic Core.

## Consequences

Positive:

- STT and TTS adapters share one small validated invocation protocol;
- recognition and synthesis remain explicit without duplicated contract trees;
- owner and authority provenance survives the external speech boundary;
- over-limit and correlation failures remain deterministic and inspectable;
- device and presentation implementation remains replaceable.

Negative:

- exact payload schemas and adapter-defined usage units are host concerns;
- generic Core does not verify audio codecs, sample rates, language tags, or
  voice compatibility;
- streaming and partial recognition require later contracts;
- current-authority freshness remains an orchestration responsibility.

## Deferred

Commit 0024 does not implement:

- concrete STT or TTS provider adapters;
- microphones, audio devices, recording, playback, codecs, or transcoding;
- endpoint URLs, credentials, HTTP transport, retries, timeouts, or rate limits;
- streaming audio, partial transcripts, or incremental synthesis;
- voice catalog discovery, language negotiation, or pronunciation dictionaries;
- lip sync, facial animation, subtitles, or presentation scheduling;
- conversation state, address resolution, or social turn-taking;
- automatic Knowledge, Memory, Relationship, Behavior, Command, or Event
  changes.

## Enforcement

Tests cover exact payload policy, explicit operations, bounded limits and usage,
stable codes, immutable request/response correlation, one adapter invocation
for recognition and synthesis, adapter mismatch preflight, explicit rejection
and failure, every completed-response mismatch, input/output limit validation,
cancellation, and exception propagation without retry.

Repository verification enforces the exact asynchronous adapter boundary, one
adapter call, explicit recognition/synthesis and completed/rejected/failed
semantics, full correlation and limit statuses, and absence of concrete
providers, network clients, hidden retries, clocks, generated IDs,
authoritative managers, model/prompt orchestration, device APIs, filesystem
I/O, game vocabulary, and public setters.
