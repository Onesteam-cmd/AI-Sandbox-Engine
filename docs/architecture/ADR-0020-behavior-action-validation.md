# ADR-0020: Typed behavior intents and pure action validation

- **Status:** Accepted
- **Date:** 2026-07-22
- **Scope:** Desired-outcome intents, concrete action proposals, pure
  validation, typed command payloads, explicit rejection, optimistic snapshot
  gates, and separation from command execution

## Context

The foundation now contains authoritative World State, deterministic commands
and ticks, subjective perception, knowledge, memory, and directed
relationships. The next boundary must let AI or deterministic behavior systems
express what an actor wants and what concrete action it proposes without
allowing generated output to mutate the world directly.

A desired outcome and an executable action are not the same thing. "Become
safer" may lead to moving, hiding, asking for help, closing a door, or doing
nothing. A concrete proposal still needs authoritative validation for current
state, actor capability, access, range, resources, social rules, and domain
policy.

The reusable Core must not define a universal action enum or game-specific
action catalogue.

## Decision

Introduce:

- exact `IBehaviorIntent` payloads for desired outcomes;
- exact `IActionProposal` payloads for concrete requested actions;
- externally assigned intent and proposal IDs;
- immutable snapshot-scoped intent and proposal envelopes;
- optional intent-to-proposal provenance through `SourceIntentId`;
- stable lowercase `ActionRejectionCode` values;
- `ActionDecision<TCommand>` with either one exact command payload or one
  explicit rejection;
- synchronous pure `IActionValidator<TState,TAction,TCommand>`;
- `ActionValidationProcessor<TState,TAction,TCommand>` with preflight and
  post-evaluation authority checks;
- explicit world, version, and logical-tick mismatch outcomes.

```text
reasoning / deterministic behavior selection
                    ↓
BehaviorIntentEnvelope<TIntent>
desired outcome, not executable
                    ↓ behavior controller
ActionProposalEnvelope<TAction>
concrete request, not authoritative
                    ↓ pure action validator
ActionDecision<TCommand>
approved command payload OR rejection code
                    ↓ explicit host orchestration
CommandEnvelope<TCommand> → Command Processor → World State
```

## Exact payload types

Intent, proposal, and approved command generic parameters must be exact value
types or sealed reference types. Interfaces, abstract classes, open generic
types, and unsealed reference classes are rejected.

Core therefore avoids string-keyed intent/action dictionaries while permitting
each domain module to define its own typed payloads. The same actor can use many
exact intent and action types without a central Core action enum.

## Snapshot coordinates

Every intent and action proposal records:

- actor entity ID;
- world ID;
- authoritative World State version;
- logical simulation tick;
- externally assigned payload ID.

A proposal derived from an intent copies those coordinates and stores the
source intent ID. External, player, simulation, or host proposals may omit a
source intent.

The processor rejects wrong-world, stale-version, and stale-tick proposals
before invoking the validator.

## Pure validation

The validator receives one stable `WorldStateSnapshot<TState>` and the exact
proposal. It runs once. It returns:

- `Approved` with one exact `IEngineCommand` payload; or
- `Rejected` with one initialized machine-readable rejection code.

The validator does not receive a command processor, runtime orchestrator, event
dispatcher, persistence transport, clock, provider, or authority mutation
capability through the contract.

The Core cannot mechanically prove arbitrary validator implementations are
pure. Repository policy, code review, module tests, and dependency injection
must enforce that concrete validators only inspect the supplied snapshot and
deterministic policy data.

## One command payload per approved action

An approved decision returns one command payload, not a command envelope and
not a list of commands. The host remains responsible for assigning a command
ID, preserving expected world/version/tick coordinates, invoking the existing
command boundary, and handling the resulting commit fact.

A domain action that requires several atomic changes must define one composite
typed command whose handler performs one atomic World State transition. This
avoids ambiguous partial execution and ordering semantics.

## Optimistic validation

The processor reads authority before validation. It invokes the validator only
when proposal coordinates match.

After validation it reads authority again. If the logical tick or World State
version changed, the decision is discarded and an explicit conflict status is
returned. The processor does not retry or invoke the validator again.

Validator exceptions propagate to the caller. They are not converted into
rejections, hidden, or retried.

## Rejection codes

`ActionRejectionCode` is an ordinal stable token of 1 to 64 lowercase ASCII
letters, digits, periods, underscores, or hyphens.

Codes are machine-readable policy outcomes, not player-facing prose. A future
dialogue or presentation layer can translate a code and current context into a
natural explanation. Core does not freeze one global rejection taxonomy.

## Separation from adjacent layers

This increment does not:

- choose behavior;
- convert every intent into an action;
- execute or enqueue commands;
- mutate World State;
- dispatch completed events;
- update Knowledge, Memory, or Relationships;
- call an LLM, STT, TTS, database, network, or game engine;
- define actor capability, navigation, inventory, acoustics, or interaction
  rules.

All cross-layer effects require explicit host orchestration.

## Alternatives considered

### One universal action enum with parameter bags

This centralizes routing but freezes game vocabulary in Core, weakens compile
time validation, and produces stringly typed parameters. Rejected.

### Let behavior or LLM output commands directly

This collapses desired outcomes, concrete actions, validation, and authority.
It makes impossible actions indistinguishable from valid commands and bypasses
natural rejection policy. Rejected.

### Let validators execute commands

This introduces hidden side effects and a time-of-check/time-of-use authority
violation. It also prevents callers from inspecting or composing decisions.
Rejected.

### Return a list of independent commands

Ordering, failure, rollback, and atomicity become ambiguous. One composite
domain command is the existing deterministic solution for multi-change
actions. Rejected.

### Store player-facing rejection text in Core

Localized and character-specific explanation belongs to dialogue and
presentation. Stable machine-readable codes are sufficient at this boundary.
Rejected.

### Automatically retry after authority changes

A second evaluation may repeat expensive reasoning or side effects in an
incorrect validator. It also hides contention. Explicit conflict without retry
matches existing Core policy. Rejected.

## Attempt to disprove the decision

One-command approval can produce broad composite commands. Domain review must
prevent composite commands from becoming unbounded transaction bags.

Exact intent and proposal types may proliferate. Later context retrieval and
behavior-controller composition need naming, versioning, and ownership
guidance.

A validator can still capture mutable services and violate purity because C#
interfaces cannot enforce referential transparency. The contract minimizes the
surface, and repository checks prohibit authority execution inside the Core
implementation, but concrete module validators still require review.

Snapshot validation does not reserve the world. A conflict can occur after an
approved decision is returned and before command execution. That is expected:
the eventual command envelope carries optimistic world/version/tick
coordinates and the command processor remains authoritative.

Rejection codes can fragment without module governance. Concrete products must
publish stable code registries and migrations without moving their vocabulary
into generic Core.

## Consequences

Positive:

- desired outcomes and executable actions are explicitly different;
- generated behavior cannot bypass action validation;
- exact typed payloads remain generic and domain-extensible;
- approval produces inspectable command data without side effects;
- rejection is explicit, stable, and machine-readable;
- stale input is rejected before validator execution;
- mid-validation authority changes discard results without retry;
- command execution and World State authority remain unchanged.

Negative:

- behavior selection and intent-to-action planning remain unimplemented;
- domains must define intent, proposal, command, and rejection schemas;
- validators require purity discipline outside what the type system can prove;
- approved decisions may still conflict before command execution;
- no registry routes heterogeneous action types yet.

## Deferred

Commit 0020 does not implement:

- a behavior planner or behavior tree;
- LLM/provider integration;
- heterogeneous action routing;
- actor capability or affordance models;
- navigation, interaction, inventory, or acoustic rules;
- command execution from approved decisions;
- natural-language refusal generation;
- context retrieval or prompt composition;
- retries, queues, reservations, or long-running actions.

## Enforcement

Tests cover exact payload policy, intent-to-proposal provenance, approved
command separation, explicit rejection, stale preflight gates, authority
conflict after one validator call, exception propagation without retry, and
ordinal rejection-code validation.

Repository verification enforces the pure processor boundary, one validator
invocation, explicit statuses, exact command output, and absence of hidden
command execution, World State mutation, providers, clocks, tasks, I/O, event
dispatch, or game-specific action vocabulary.
