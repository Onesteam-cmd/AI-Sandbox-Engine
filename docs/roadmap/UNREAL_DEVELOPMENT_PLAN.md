# Unreal integration plan

## Purpose

`AI-Sandbox-Detective` is the first Unreal Engine 5 consumer of AI Sandbox
Engine. It is maintained as a separate repository so generic Core remains
headless, reusable, provider-neutral, and independent from Unreal types.

## Separation

```text
Unreal Engine 5
↕ local bridge
AI.Sandbox.Host.exe
↕
AI Sandbox Engine Core
↕
LLM / STT / TTS / persistence adapters
```

Core remains a set of .NET assemblies consumed through a separate Host. This
keeps headless simulation testable, isolates provider and game-engine failures,
and preserves the ability to replace adapters independently.

## Development bridge

The Unreal project uses a dedicated development/integration bridge composed of:

- a C++ runtime bridge between Unreal and the .NET Host;
- editor-side tooling for controlled project and level automation;
- Unreal Python utilities for repeatable content operations;
- PowerShell orchestration for build, test, verification and report collection.

These tools belong to the game repository and do not introduce Unreal
dependencies into generic Core.

## Integration workflow

```text
bounded integration change
→ apply/build
→ automated checks
→ runtime or editor validation
→ collect diagnostics
→ review result
→ next bounded change
```

Game-side tooling may automate map construction, semantic zones, navigation,
collision, materials, lighting, asset import, Map Check, Automation Tests and
repeatable playtest diagnostics.

## Vertical-slice direction

The first product slice connects a small playable location to Core systems for:

- NPC identity and state;
- conversation and semantic addressee resolution;
- social turn-taking;
- Knowledge, Memory and Relationships;
- provider-neutral model/speech integration through Host adapters;
- validated physical NPC actions.

Location and presentation work expands on the game side without changing Core
unless an actual missing engine capability is discovered.

## Core coordination

Core development reached the terminal `0094 Core Product Pipeline Completion`
gate. Unreal integration therefore treats Core as a stable dependency.

A later Core change requires one of the following:

- a reproducible integration blocker;
- a correctness defect;
- a missing production capability with a concrete consumer;
- a measurable product requirement that cannot be solved at the adapter, Host,
  gameplay, or presentation layer.
