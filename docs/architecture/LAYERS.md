# Architectural layers

Canonical conceptual order:

```text
Infrastructure → Data → Runtime → Simulation → AI → Gameplay → Presentation
```

This list describes increasing specialization. Concrete project references will be introduced one commit at a time and validated automatically.

## Permanent boundary

The reusable engine consists of generic infrastructure, data, runtime, simulation, and AI-facing contracts.

Gameplay and presentation may depend on the engine. The engine must never depend on a detective case, quest, dialogue UI, Living World scenario, Unreal type, Unity type, or presentation object.

## World-state authority

All authoritative changes follow this shape:

```text
External input or simulation event
→ intent or proposed action
→ validation
→ authoritative state transition
→ emitted domain events
```

An LLM response is external untrusted input. It may propose an action but cannot assert that the action happened.

## Integration rule

Provider-specific code belongs in adapters. Examples:

- LLM provider adapters;
- STT and TTS adapters;
- database adapters;
- Unreal or Unity bridges;
- telemetry exporters.

Core projects depend on contracts, not provider SDKs.

## Determinism rule

Where randomness affects authoritative simulation, it must be supplied through an explicit deterministic random source whose seed and decisions can be reproduced.

Wall-clock time, environment variables, network responses, and global random generators must not silently determine authoritative simulation outcomes.
