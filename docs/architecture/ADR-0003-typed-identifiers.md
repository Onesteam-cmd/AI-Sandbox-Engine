# ADR-0003: Generic strongly typed identifiers

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Foundational identity representation

## Context

Raw `Guid`, integer, and string values allow identifiers belonging to unrelated
concepts to be mixed accidentally. That failure is especially dangerous in an
event-driven simulation where entities, worlds, events, memories, jobs, and
other records will cross many subsystem boundaries.

The representation must remain reusable, allocation-free as a value, independent
of gameplay, and compatible with deterministic simulation and future
persistence.

## Decision

Use the dependency-free generic value type:

```csharp
Id<TKind>
```

`TKind` is a phantom compile-time marker. It is not instantiated or stored.
Consequently, `Id<EntityKind>` and `Id<WorldKind>` are incompatible closed CLR
types even though both contain one `Guid`.

The identifier:

- stores exactly one `Guid`;
- reserves `Guid.Empty` as the uninitialized default value;
- accepts valid values only through `From` or strict `Parse`;
- parses and formats canonical GUID `D` text;
- compares and hashes by the wrapped GUID within one identifier kind;
- exposes the underlying GUID for explicit infrastructure boundaries;
- performs no ID generation.

Generation is deliberately separated from representation. Future runtime code
will receive an explicit identity source. Deterministic tests, replays, and
authoritative simulations can then supply reproducible identifiers instead of
silently depending on process-global randomness or wall-clock state.

The canonical text does not contain a CLR type name or prefix. The surrounding
schema or typed API determines the identifier kind. This keeps persisted values
stable if namespaces or implementation type names change.

## Alternatives considered

### Raw GUIDs everywhere

This is simple but permits an entity ID to be passed where a world ID is
expected. Rejected because the compiler cannot protect subsystem boundaries.

### Dedicated wrapper struct for every identifier

This provides excellent call-site names but repeats validation, parsing,
formatting, and serialization behavior across many types. It also creates a
large maintenance surface. Rejected as the foundation representation.

### String identifiers with prefixes

Human-readable prefixes can help diagnostics but introduce allocations,
parsing policy, casing questions, and coupling between persistence and symbolic
type names. Rejected for the core representation. Presentation layers may add
labels without changing identity.

### ULID or another external identifier package

Alternative formats may offer useful ordering properties, but adding a package
would violate the dependency-free core boundary and make its API foundational.
Rejected unless later measurements prove GUIDs insufficient.

### Internal random generation on `Id<TKind>`

A `New()` method is convenient but hides a nondeterministic dependency inside a
value type. That would make replay and deterministic simulation harder to
enforce. Rejected.

### Source-generated dedicated IDs

A generator could combine concise names with shared behavior, but it adds build
complexity before there is evidence that generic call sites are problematic.
Deferred rather than selected.

## Attempt to disprove the decision

Generic identifiers are more verbose than dedicated names and the default value
of any struct cannot be prevented. The design addresses the second issue by
making emptiness explicit and rejecting it at every creation and parsing
boundary.

The decision would be invalid if profiling showed that GUID size, comparison, or
text conversion materially harms the target simulation, or if integrations
cannot preserve the generic kind safely. Those conditions are not currently
demonstrated. The underlying value remains explicit, so a later migration can be
designed without contaminating gameplay systems.

## Consequences

Positive:

- compile-time separation between unrelated identifiers;
- no heap allocation for identifier values;
- no package dependency;
- stable canonical persistence text;
- deterministic generation remains injectable;
- one tested implementation for every future identifier kind.

Negative:

- generic type names are more verbose at API boundaries;
- `default(Id<TKind>)` exists and must be validated;
- a heterogeneous untyped data store must carry kind information separately;
- creation requires an explicit GUID or a future identity source.

## Enforcement

Tests cover emptiness, equality, hashing, parsing, canonical formatting,
ordering, and closed-generic type separation. Repository verification rejects
hidden `Guid.NewGuid()` or `Guid.CreateVersion7()` calls inside the primitive and
continues enforcing the dependency-free core boundary.
