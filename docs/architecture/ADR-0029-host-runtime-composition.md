# ADR-0029: Host Runtime Composition Contracts

- **Status:** Accepted
- **Date:** 2026-07-22
- **Commit:** `0029 Host Runtime Composition Contracts`

## Context

Independent Core boundaries now exist for simulation, subjective state,
dialogue, model and speech adapters, structured output, and action validation.
The external `.NET Host` needs an explicit dependency graph without moving a DI
container, lifecycle side effects, transport, provider SDKs, or Unreal
dependencies into generic Core.

## Decision

Commit 0029 adds exact immutable capability payloads, stable capability and
composition IDs, bounded explicit dependencies, deterministic topological
ordering, and explicit results for empty input, excessive input, duplicate IDs,
missing dependencies, and cycles.

Composition produces a validated startup plan only. It never constructs, starts,
stops, disposes, resolves, or invokes capabilities.

## Invariants

1. Capability and composition IDs are externally assigned and non-empty.
2. Payloads are exact value types or sealed reference types.
3. Dependencies are initialized, unique, bounded to 32, and not self-references.
4. Compositions contain at most 128 capabilities.
5. Every dependency exists in the same composition.
6. Successful order is deterministic and dependency-safe.
7. Independent ready nodes use stable capability-ID ordering.
8. Composition performs no lifecycle work, I/O, retry, or scheduling.
9. Core remains independent of DI containers, providers, transport, and Unreal.

## Deferred

Concrete capability construction, startup/shutdown, health, restart policy,
bridge transport, providers, background services, and Unreal registration remain
external Host responsibilities.

## Enforcement

Tests and repository verification cover bounds, exact payload policy,
immutability, duplicate IDs, missing dependencies, cycles, deterministic order,
stable tie-breaking, and absence of lifecycle or integration side effects.
