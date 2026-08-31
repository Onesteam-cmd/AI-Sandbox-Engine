# Foundation Validation 0010

## Purpose

This document records the acceptance criteria for the first frozen
AI Sandbox Engine foundation.

## Validation matrix

| Concern | Validation |
|---|---|
| Typed identity | Stable externally supplied world, entity, system, and event IDs |
| Event runtime | Post-commit exact-type handlers execute in registration order |
| World State | One authoritative immutable snapshot and versioned transitions |
| Entity lifecycle | Active entity destruction retains a permanent tombstone |
| Components | Typed data is purged on destruction and remains lifecycle-consistent |
| Scheduler | Fixed system order, one version and one logical tick per step |
| Persistence | Metadata and payload survive save/restore continuation |
| Determinism | Uninterrupted and resumed execution produce identical payload/checksum |
| Construction order | Reversed initial insertion order produces identical snapshot |
| Headless execution | Console probe runs without presentation or game-engine adapters |
| Performance guardrail | 5,000 ticks complete below the broad 30-second ceiling |

## Standard command

```powershell
& .\eng\validate-foundation.ps1
```

A faster local check after an existing Release build:

```powershell
& .\eng\validate-foundation.ps1 -NoBuild -RepeatCount 2
```

## Interpretation

A passing result means the foundation layers compose according to their frozen
contracts. It does not mean the engine has final gameplay, AI, networking, game
integration, or production-scale performance.

The next development phase may build runtime commands and higher simulation
systems on this foundation. Changes to frozen semantics require an ADR and must
keep this validation green.
