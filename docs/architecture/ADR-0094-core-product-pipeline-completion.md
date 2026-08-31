# ADR-0094: Core Product Pipeline Completion

- Status: Accepted
- Date: 2026-08-08

## Context

After `0093 Prompt Composition Foundation Probe`, a bounded Core completion audit
found no concrete production-domain implementation gap that justified another
contract, recovery mechanism, queue, scheduler, or host-runtime layer.

The remaining uncertainty was integration-shaped: the repository had executable
proofs for individual subsystems, but no single proof that the existing Core
could carry one product-shaped AI turn from retrieved context through prompt
composition and provider-neutral inference to a validated authoritative command.

A transient `0094` probe was therefore built first. It compiled with zero
warnings/errors, preserved the full 782-test baseline, executed the complete
pipeline successfully, and rolled back to the exact clean `0093` predecessor.

## Decision

Persist exactly one FoundationProbe consumer path:

1. retrieve bounded subjective context;
2. compose a budgeted prompt;
3. invoke the existing provider-neutral model boundary through a deterministic
   fake adapter;
4. decode a structured response;
5. validate the proposed action;
6. execute the resulting command through the existing runtime authority path.

All inference stages must leave authoritative World State unchanged. The only
state mutation in the scenario is the final accepted command, which advances
World State version exactly once.

No production file under `src/AI.Sandbox.Engine.Core` is added or modified by
this increment.

## Core completion gate

With this product-shaped consumer path green together with the existing
FoundationProbe scenarios and complete unit-test baseline, the Core foundation
is considered complete for Game integration.

Further Core work is prohibited by default. A new Core increment requires a
specific Game/Host integration failure, correctness bug, missing production
capability, or measurable product requirement that cannot be solved at the
consumer/integration layer.

The following are explicitly not reasons to reopen Core development:

- adding another recovery layer for theoretical completeness;
- adding more wrappers around already executable processors;
- creating parallel transports, queues, schedulers, or lifecycle abstractions
  without a demonstrated consumer requirement;
- extending probes only to increase subsystem coverage counts.

## Consequences

Development now leaves Core-first mode. The next phase is manual Game/Unreal
integration with the Core treated as a stable dependency.

The deterministic fake adapter in the probe is validation scaffolding only; it
does not prescribe the real Game provider implementation.
