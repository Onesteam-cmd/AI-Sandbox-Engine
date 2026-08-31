# ADR-0017: Owner-scoped current subjective knowledge

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Subjective claims, confidence, evidence provenance, current
  revisions, explicit acquisition, optimistic revision, removal, and
  persistence

## Context

Perception produces observer-specific signals, but a signal is transient
evidence. The engine needs a durable current epistemic layer without copying
objective World State into every entity or confusing current beliefs with
episodic memory.

## Decision

Introduce exact claim types, typed claim and evidence IDs, fixed-point
confidence, explicit evidence provenance, current positive revisions, and
owner-scoped `KnowledgeSet<TClaim>` components.

```text
World State / Event
objective authority
        ↓
Perception / communication / inference
subjective evidence
        ↓ explicit command or system
Knowledge Entry
owner-scoped current claim
        ↓
future memory, reasoning, and behavior
```

A claim may be false, stale, incomplete, or contradictory. Its presence never
changes objective World State truth.

## Current state, not memory history

A knowledge entry retains the current claim, confidence, latest evidence, first
acquisition metadata, and current revision. It does not retain earlier
revisions, episodes, salience, rehearsal, decay, forgetting, or recall.

Those mechanisms belong to `0018 Memory Model`.

## Evidence

Evidence records the recipient, world, exact version and tick, broad source
kind, optional source entity, and optional perception stimulus/channel
provenance.

Perception evidence is created from an observation. Communication evidence
requires a source entity. Evidence must match the set owner and world.

## Revisions

Add creates revision 1. Revision and removal require an exact expected revision.
Older evidence is rejected. Identical claim, confidence, and evidence produce
`Unchanged`. Successful revision uses checked arithmetic.

## Persistence

Restore validates positive revisions, concrete claim types, non-zero active
confidence, first acquisition ordering, unique IDs, evidence ownership, world
scope, and deterministic claim-ID ordering.

Concrete modules remain responsible for serializing their claim payloads.

## Explicit acquisition

Perception remains read-only. An observation changes no knowledge until a
validated command or simulation system interprets it into a domain claim and
replaces the immutable component through World State.

This permits lies, misperception, stale beliefs, and different interpretations
without corrupting objective authority.

## Deferred

Commit 0017 does not implement contradiction logic, multi-source evidence
aggregation, inference engines, memory episodes, decay, forgetting, recall,
relationships, behavior, prompts, or providers.

## Consequences

Positive:

- subjective worldviews are explicit and owner-scoped;
- perception does not create knowledge automatically;
- source provenance and confidence are retained;
- stale revisions cannot overwrite current claims;
- persistence is deterministic;
- memory remains a separate architecture layer.

Negative:

- modules define claim payloads and identity policy;
- only latest evidence is retained;
- contradiction detection is domain-specific;
- acquisition requires explicit commands or systems.

## Enforcement

Tests cover confidence, evidence invariants, perception provenance, exact claim
types, owner/world scope, deterministic ordering, revision conflict, evidence
regression, unchanged behavior, removal, explicit acquisition, and
byte-identical save/restore continuation.
