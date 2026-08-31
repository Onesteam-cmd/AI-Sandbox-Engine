# Current project status

- **Core status:** complete for Game integration
- **Terminal Core gate:** `0094 Core Product Pipeline Completion`
- **Development mode:** integration-driven; no automatic continuation of the Core roadmap
- **Game integration:** maintained separately in `AI-Sandbox-Detective`
- **Foundation status:** integrated, validated and frozen unless a concrete blocker is demonstrated

## Product pipeline

The persistent FoundationProbe composes the existing production boundaries into
one product-shaped path:

```text
Context Retrieval
→ Prompt Budget / Composition
→ provider-neutral Model Invocation
→ Structured Output
→ Action Validation
→ Runtime Command
```

Inference-side stages preserve authoritative World State. The final accepted
command is the only state mutation in the scenario and advances the World State
version exactly once.

## Validation baseline

The `0094` completion record documents a green **782-test baseline**, successful
FoundationProbe execution and the dedicated Core completion validator.

Standard validation entry points:

```powershell
& .\eng\test.ps1
& .\eng\validate-foundation.ps1
& .\eng\validate-core-completion.ps1
```

## Change gate

Core should be reopened only for a reproducible integration blocker, correctness
defect or missing production capability. New recovery/contract layers are not
added solely to extend the roadmap or increase abstraction depth.
