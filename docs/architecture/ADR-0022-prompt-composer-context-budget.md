# ADR-0022: Prompt Composer and Context Budget Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0022 Prompt Composer and Context Budget Contracts`

## Context

Context Retrieval now returns bounded, owner-scoped, exact typed items from a
stable authoritative snapshot. The next boundary must select a minimal useful
subset and compose a provider-ready logical document without dumping the world,
calling a model provider, performing retrieval again, or mutating authoritative
or subjective state.

Provider tokenizers are not stable Core concepts. Different providers and model
versions count tokens differently, while non-text future modalities may use
other capacity measures. Core therefore needs deterministic provider-neutral
budget units whose concrete estimation policy is supplied by the host.

## Decision

Introduce exact typed prompt requests, candidates, documents, deterministic
budget allocation, and a pure prompt-composition processor.

A `PromptRequestEnvelope<TRequest>` carries external identity, subjective owner,
world/version/tick coordinates, a positive provider-neutral budget, and one
exact request payload.

A `PromptCandidateEnvelope<TContent>` carries external identity, owner, world,
required-or-optional inclusion, deterministic priority, positive estimated
cost, and one exact content payload. Candidates are supplied by orchestration;
the Prompting layer does not invoke Context Retrieval.

`PromptBudgetManager<TContent>` is stateless and deterministic:

1. candidate IDs must be unique;
2. required candidates sort before optional candidates;
3. candidates within a mode sort by priority descending and stable ID ascending;
4. every required candidate must fit or allocation returns
   `RequiredBudgetExceeded`;
5. optional candidates are greedily included in deterministic order whenever
   their cost fits the remaining budget.

An exact `IPromptComposer<TState,TRequest,TContent,TDocument>` receives one
stable snapshot, the exact request, the successful immutable budget result, and
its stable composer ID. It is invoked at most once and returns one exact prompt
document or one stable rejection code.

`PromptCompositionProcessor` performs snapshot preflight, candidate scope and
identity validation, deterministic budgeting, one pure composer invocation,
post-composition authority validation, and document scope/composer/budget
validation. It never calls a model provider or applies a command.

## Alternatives considered

### Store a final string prompt directly in Core

Rejected. It would freeze presentation and provider formatting, lose structured
payload contracts, and make future multimodal composition difficult.

### Define all budgets as provider token counts

Rejected. Token counts depend on tokenizer and provider version. Core uses
host-defined deterministic units; adapters may later estimate those units with
a concrete tokenizer outside authoritative contracts.

### Let the composer retrieve context itself

Rejected. It hides source selection, risks repeated expensive retrieval, and
makes snapshot provenance and invocation counts difficult to audit.

### Truncate required content when it does not fit

Rejected. Silent truncation can remove safety, identity, or current-state
instructions. Required overflow is explicit and composition does not start.

### Automatically summarize or compress over-budget candidates

Rejected for this increment. Summarization may require a model provider and can
change meaning. Future explicit preprocessing may produce new candidates with
new external IDs and costs.

### Execute the provider call from the composition processor

Rejected. Composition is a pure deterministic preparation boundary. Provider
execution, retries, latency, and transport belong to later adapters.

## Disproof and failure analysis

Greedy optional selection is not globally optimal for every knapsack objective.
It is selected because it is simple, bounded, deterministic, and auditable.
Hosts can assign priority and estimated cost or pre-compose alternative
candidates. A future policy interface may be added only if real workloads show
that the stable greedy policy is inadequate.

Provider-neutral cost estimates can diverge from real provider token counts.
The final document therefore also carries an estimated cost and is validated
against the request budget. Provider adapters must still enforce their actual
limits and report provider-specific rejection separately.

A prompt can become stale after composition and before inference. The document
does not grant authority. Later inference and action execution must retain
snapshot provenance and revalidate before any World State mutation.

This increment does not merge heterogeneous candidate payload types. Explicit
orchestration may compose separate exact content schemas or introduce a sealed
domain union outside generic Core.

## Consequences

Positive:

- owner/world/version/tick scope remains explicit;
- required content cannot be silently dropped;
- optional selection is bounded and deterministic;
- exact prompt schemas remain provider-neutral and extensible;
- one composer invocation is auditable;
- result owner, world, composer, and budget are validated;
- retrieval, provider calls, and authoritative mutation remain separate.

Negative:

- hosts must estimate provider-neutral costs;
- one processor handles one exact request/content/document type combination;
- greedy selection is not a general optimization solver;
- concrete rendering, tokenization, and provider execution remain deferred.

## Deferred

Commit 0022 does not implement:

- LLM, STT, or TTS provider contracts or adapters;
- tokenizer-specific counting;
- provider transport, retries, rate limits, or streaming;
- automatic retrieval or heterogeneous retriever orchestration;
- automatic summarization, compression, or embeddings;
- conversation state, address resolution, or social turn-taking;
- World State, Knowledge, Memory, or Relationship mutation;
- prompt caching or background work.

## Enforcement

Tests cover exact payload policy, bounded value objects, immutable scoped
envelopes, deterministic required/optional selection, stable tie-breaking,
required-budget overflow, stale preflight gates, candidate scope and duplicate
validation, one composer invocation, result scope and budget validation,
authority-conflict discard, explicit rejection, and exception propagation
without retry.

Repository verification enforces exact generic constraints, deterministic
selection source patterns, one composer call, pre/post authority reads, all
explicit statuses, and absence of retrieval, providers, tokenizers, mutation,
event dispatch, clocks, hidden tasks, I/O, game vocabulary, or public setters.
