# ADR-0023: Provider-Neutral Model Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0023 Provider-Neutral Model Contracts`

## Context

Prompt Composition now produces exact typed documents with explicit owner,
world, version, tick, composer, and budget provenance. The next boundary must
allow infrastructure adapters to invoke language or other generative models
without leaking provider SDKs, endpoint details, credentials, retry policy, or
provider-specific response objects into generic Core.

Model inference is long-latency external I/O and is not authoritative. A model
response may arrive after the simulated world has advanced. Core therefore must
preserve correlation coordinates and validate adapter output, while leaving any
later decision to use or discard stale content to explicit orchestration and
Action Validation.

## Decision

Introduce exact typed provider-neutral request and response payload markers,
opaque adapter and model-profile IDs, externally assigned invocation IDs,
provider-neutral output limits and usage values, one asynchronous adapter
interface, and a validating invocation processor.

A `ModelInvocationRequestEnvelope<TRequest>` carries:

- invocation, adapter, model-profile, and source prompt-document IDs;
- subjective owner and authoritative world/version/tick provenance;
- one positive adapter-defined output limit;
- one exact immutable request payload.

A completed `ModelInvocationResponseEnvelope<TResponse>` echoes every
correlation coordinate, reports initialized non-negative input/output usage in
adapter-defined units, and carries one exact immutable response payload.

`IModelAdapter<TRequest,TResponse>` is the only explicit external invocation
boundary in this increment. Implementations live outside generic Core. The
Core processor calls the adapter at most once, passes an explicit cancellation
token, performs no retry, timeout, transport, credential, or logging policy,
and propagates adapter exceptions and cancellation.

Adapters return one `ModelInvocationDecision<TResponse>`:

- `Completed` with one response;
- `Rejected` with a stable machine-readable rejection code;
- `Failed` with a stable machine-readable operational failure code.

`ModelInvocationProcessor` rejects a request aimed at another adapter before
invocation. For completed decisions it validates invocation, adapter, model
profile, source prompt, owner, world, version, tick, and output limit. It does
not read `WorldStateManager`, mutate state, execute commands, dispatch events,
or automatically accept model content as fact, memory, relationship, intent,
or action.

## Alternatives considered

### Put concrete provider SDK request and response types in Core

Rejected. It would reverse the dependency direction, couple the foundation to
vendor release cycles, and make provider replacement or local adapters costly.

### Use provider names and model names as authoritative strings

Rejected. Hosts may map opaque stable profile IDs to changing provider, model,
region, quantization, or deployment configuration without changing Core state
contracts or persistence schemas.

### Define one universal text request and response DTO

Rejected. Exact generic payload types preserve structured output and allow
future text, vision, speech, or multimodal adapters without a closed modality
enum or nullable union fields.

### Add automatic retries, backoff, timeouts, or rate limiting

Rejected. Those policies depend on provider semantics and host operations.
Hidden retries also violate at-most-once auditability. Infrastructure may build
an explicit higher-level policy around separate invocation IDs.

### Bind the processor to WorldStateManager and reject any response after a tick

Rejected. External inference can legitimately cross many ticks. The response is
non-authoritative and retains its source coordinates. Later orchestration must
choose freshness policy and revalidate before any authoritative command.

### Add streaming output now

Deferred. Streaming requires chunk identity, ordering, partial failure,
cancellation, and presentation semantics. The bounded completed-response
contract should be validated before adding that surface.

## Disproof and failure analysis

An adapter can misreport usage or correlation fields. The processor rejects
correlation mismatches and output-limit overflow, but cannot independently
verify provider metering. Concrete infrastructure must enforce real provider
limits and observability.

Opaque model-profile IDs reduce accidental provider coupling but require host
configuration and migration discipline. That is intentional: model selection
is deployment policy, not authoritative simulation vocabulary.

Exact request and response types can increase adapter count. This cost buys
compile-time schema separation and prevents one permissive dictionary contract
from becoming an unvalidated cross-layer data channel.

The processor does not check current authority after inference. This is not an
acceptance of stale output. Model output has no authority; any action, knowledge,
memory, or relationship update still requires explicit current-state
validation through later orchestration.

## Consequences

Positive:

- provider SDKs and names remain outside Core;
- invocation and prompt provenance remain explicit;
- exact structured request and response schemas are supported;
- one adapter call is auditable and cancellable;
- rejection, operational failure, and completed-response validation are
  distinct;
- response correlation and output limits are validated;
- model output cannot mutate or become truth automatically.

Negative:

- infrastructure must map opaque profile IDs to concrete deployments;
- usage units are adapter-defined and not globally comparable;
- retries, streaming, rate limits, and observability require later explicit
  infrastructure;
- freshness policy remains an orchestration responsibility.

## Deferred

Commit 0023 does not implement:

- concrete OpenAI, Anthropic, Gemini, local-model, or other provider adapters;
- endpoint URLs, credentials, HTTP transport, retries, timeouts, or rate limits;
- tokenizers or provider-specific token accounting;
- streaming chunks or partial output;
- speech-to-text or text-to-speech contracts;
- response schema repair or automatic re-prompting;
- conversation state, address resolution, or social turn-taking;
- automatic Knowledge, Memory, Relationship, Behavior, Command, or Event
  changes.

## Enforcement

Tests cover exact payload policy, bounded limits and usage, stable codes,
immutable request/response correlation, one adapter invocation, adapter mismatch
preflight, explicit rejection and failure, every completed-response mismatch,
output-limit validation, cancellation, and exception propagation without retry.

Repository verification enforces the asynchronous exact adapter boundary, one
adapter call, explicit completed/rejected/failed decisions and validation
statuses, prompt-document provenance, and absence of concrete providers,
network clients, retries, clocks, generated IDs, authoritative managers,
commands, events, subjective-store mutation, game vocabulary, and public
setters.
