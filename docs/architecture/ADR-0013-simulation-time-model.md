# ADR-0013: Integer fixed-step simulation time model

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Deterministic duration, internal epoch time, fixed tick duration,
  boundary conversion, deadlines, persistence, and command semantics

## Context

The Scheduler currently advances an unsigned logical tick. Future needs,
movement, memory decay, work schedules, travel, weather, economy, and behavior
all require durations and due times.

Using rendering delta time, `DateTime`, `TimeSpan`, wall-clock timestamps, or
floating-point accumulated seconds would make authoritative results depend on
host pacing, timezone rules, process suspension, or rounding history.

The time model must map the already-authoritative logical tick to exact
deterministic time, preserve the distinction between commands and ticks, and
remain small enough to persist directly in World State.

## Decision

Introduce:

- `SimulationDuration` — non-negative integer microseconds;
- `SimulationInstant` — integer microseconds since an internal world epoch;
- `SimulationTickDuration` — positive fixed microseconds per logical tick;
- `SimulationTimeline` — exact tick/instant and deadline mapping.

The internal epoch is simply simulation instant zero. It is not UTC, local time,
a real-world calendar date, or a wall-clock timestamp.

All authoritative calculations use `ulong` microseconds and checked integer
arithmetic.

## Why microseconds

Microseconds provide:

- exact millisecond, second, minute, hour, and 24-hour-day conversion;
- adequate precision for simulation and gameplay;
- approximately 584,000 years of non-negative range in `ulong`;
- no floating-point accumulation;
- compact persistence.

Sub-microsecond physics remains an engine-adapter concern and must not redefine
authoritative social-simulation time.

## Tick mapping

For a fixed positive tick duration `D`:

```text
instant(tick) = tick * D
```

The multiplication is checked. A timeline does not read or own a clock; it only
maps explicit logical ticks.

At 50 milliseconds per tick:

- tick 0 = 0 microseconds;
- tick 1 = 50,000 microseconds;
- tick 20 = 1,000,000 microseconds.

The Scheduler remains the authority that advances tick. Systems can derive the
target instant from `context.TargetSimulationTick`.

## Boundary conversion

`GetTickAtOrBefore(instant)` uses floor division.

`GetFirstTickAtOrAfter(instant)` uses ceiling division. This is the standard
mapping for deadlines that cannot execute between ticks.

A delay from tick 10 with a 50-millisecond tick duration maps as follows:

- 0 microseconds → tick 10;
- 1 microsecond → tick 11;
- 50,000 microseconds → tick 11;
- 50,001 microseconds → tick 12.

No fractional tick exists in authoritative state.

## Commands

Accepted commands increment World State version but preserve logical tick.
Therefore they also preserve the tick-derived simulation instant.

Example:

- scheduler commit: version 1, tick 1, instant 50 ms;
- accepted command: version 2, tick 1, instant 50 ms;
- scheduler commit: version 3, tick 2, instant 100 ms.

Input volume cannot accelerate world time.

## Persistence

A world schema persists:

- the fixed tick duration;
- any materialized current instant or deadlines required by its state;
- the authoritative snapshot tick already stored by the persistence envelope.

On decode, game codecs must validate consistency between materialized time and
the snapshot tick when appropriate.

The Core timeline is transport-independent and performs no serialization itself.

## Calendar and presentation separation

Calendar concepts such as year, season, month, weekday, hour labels, time zones,
and localized text are not part of this commit.

A future calendar layer may deterministically map `SimulationInstant` to
fictional or real-style calendar fields. Presentation adapters may display them.
Neither layer may use wall-clock time as authority.

## Alternatives considered

### Floating-point seconds

Convenient but accumulates rounding error and can diverge across operation order.
Rejected.

### `TimeSpan`

Uses integer ticks internally, but imports a framework-specific unit and API,
permits negative values, and encourages wall-clock conflation. Rejected for the
authoritative domain primitive.

### `DateTimeOffset`

Represents civil wall-clock time and timezone offsets rather than elapsed world
simulation time. Rejected.

### Variable tick duration

Complicates replay, deadline mapping, persistence, and system reasoning.
Time-speed changes belong to host pacing, not authoritative tick duration.
Rejected for the foundation.

### Store only instants and remove logical ticks

The Scheduler and World State already use ticks as exact ordering and commit
boundaries. Instants complement ticks; they do not replace them. Rejected.

### Nanoseconds

Provide unnecessary precision while reducing total representable world history
to roughly 584 years. Rejected.

### Milliseconds

Provide a much larger range but unnecessarily constrain short deterministic
durations. Microseconds are the better balance.

## Attempt to disprove the decision

A fixed tick duration means an action due one microsecond after a boundary waits
almost a complete tick. This is inherent to fixed-step simulation. Higher
precision requires a shorter tick duration or a subsystem with deterministic
substeps.

`ulong` cannot represent time before the internal epoch or negative durations.
Historical calendars can use their own signed display offset above this elapsed
time model without changing authority.

Materializing current instant in state duplicates information derivable from
tick and timeline. Some worlds may choose not to store it. The integration tests
store it only to prove Scheduler and persistence composition.

Changing tick duration after world creation would reinterpret all ticks and
deadlines. Commit 0013 treats it as stable world configuration. A future
migration must explicitly transform persisted time data.

## Consequences

Positive:

- exact deterministic elapsed world time;
- no wall-clock or rendering dependency;
- no floating-point drift;
- direct tick, duration, instant, and deadline conversion;
- commands cannot advance world time;
- compact persistence;
- sufficient range for long-running simulated societies;
- fixed semantics across platforms.

Negative:

- no calendar or localized display model yet;
- no variable authoritative tick duration;
- no negative durations or pre-epoch instants;
- deadline precision is limited to tick boundaries;
- world schemas must deliberately persist time configuration.

## Enforcement

Tests verify exact unit conversion, checked and non-negative duration arithmetic,
instant arithmetic, positive tick duration, known fixed-step mappings, floor and
ceiling boundaries, deadline rounding, invalid and overflow behavior, Scheduler
target-tick integration, command time preservation, and byte-identical
save/restore continuation.

Repository verification enforces immutable integer microseconds, checked
arithmetic, positive tick duration, exact floor and ceiling mapping, and absence
of `DateTime`, `TimeSpan`, floating-point types, wall clocks, hidden execution,
I/O, providers, and game-domain vocabulary.
