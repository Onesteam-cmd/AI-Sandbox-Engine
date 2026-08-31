# ADR-0015: Hierarchical places with integer local positions

- **Status:** Accepted
- **Date:** 2026-07-21
- **Scope:** Stable place identity, containment hierarchy, local coordinates,
  directed connections, authoritative entity position, and engine boundaries

## Context

Perception, hearing, travel, routines, interaction, inventory access, and social
simulation all require a shared answer to:

- where an entity is;
- which room, building, district, or larger place contains it;
- whether two places are directly connected;
- how far apart two local points are.

Using Unreal transforms, navigation meshes, physics queries, or floating vectors
inside Core would make the reusable simulation depend on one presentation
engine, loaded geometry, frame timing, and platform-specific floating behavior.

A purely flat coordinate system is also insufficient. The same exact local
coordinates can occur in different rooms, buildings, vehicles, or generated
world cells. Social reasoning usually needs semantic containment before
centimeter-level geometry.

## Decision

Introduce:

- `SpatialPlaceIdKind` — stable typed place identity;
- `SpatialDistance` — non-negative integer millimeters;
- `SpatialPoint` — bounded signed local X/Y/Z millimeters;
- `SpatialPosition` — authoritative entity component containing one place and
  one local point;
- `SpatialPlace` — place identity and optional immediate parent;
- `SpatialConnection` — one directed connection and deterministic distance;
- `SpatialTopologyBuilder` — validates and freezes hierarchy and connections;
- `SpatialTopology` — immutable deterministic query model.

The model has two explicit levels:

```text
semantic place hierarchy
city → building → floor → room
                 +
local integer point inside one place
```

No name, room type, ownership, gameplay tag, or presentation transform belongs
to the generic spatial primitive.

## Place hierarchy

Every place has one stable ID and zero or one immediate parent.

The hierarchy is a forest:

- multiple roots are allowed;
- missing parents are rejected;
- self-parenting is rejected;
- containment cycles are rejected;
- destroyed or unloaded presentation objects do not change place identity.

`IsContainedWithin` includes equality. A room is within itself, its floor, its
building, and its district.

Ancestor enumeration is deterministic from immediate parent to root.

## Local coordinates

`SpatialPoint` uses signed integer millimeters relative to its containing place.

Coordinates are bounded to plus or minus one trillion millimeters per axis. The
bound is far larger than ordinary local spaces while allowing exact squared
three-axis distance calculations with `Int128` and `UInt128` without overflow.

Distance comparisons use squared Euclidean distance:

```text
dx² + dy² + dz² <= radius²
```

No square root or floating-point conversion is required.

Local distance is only defined when two `SpatialPosition` values share the same
place. Positions in different rooms or world cells require topology,
presentation geometry, or a future traversal model.

## Connections

Connections are directed. A doorway, road, elevator, portal, or one-way passage
can therefore be represented without assuming symmetry.

`AddBidirectionalConnection` is a convenience that atomically creates two
opposite directed connections with equal distance.

Distance is descriptive topology data. It does not imply:

- current traversability;
- door state;
- movement permission;
- movement speed;
- travel duration;
- acoustic transmission;
- visibility.

Those policies require dynamic world state and separate systems.

## Determinism

Builders accept places and connections in any registration order.

At build time:

- places are sorted by stable place ID;
- directed connections are sorted by origin then destination;
- duplicate IDs and directed endpoints are rejected;
- hierarchy and endpoint integrity are validated;
- exposed collections are read-only.

Equivalent topology therefore persists identically regardless of construction
order.

## Entity integration

`SpatialPosition` is an immutable exact-type component.

A command or simulation system may propose a new position only through the
existing Component Registry and World State authority. Core does not move
presentation actors or perform collision resolution.

An adapter may translate:

```text
Spatial place + local millimeters
        ↕
Unreal level/actor transform
```

The adapter is not authoritative. If presentation geometry disagrees with World
State, the host must reconcile it through validated commands.

## Deferred systems

Commit 0015 intentionally does not implement:

- route planning or shortest path;
- dynamic blocked/open edges;
- movement speed and travel time;
- collision and physics;
- line of sight;
- hearing attenuation;
- spatial indexing;
- world streaming;
- coordinate transforms between nested places;
- Unreal or Unity adapters.

Pathfinding cannot be correct until traversal permissions, dynamic blockers,
cost policy, and agent capabilities are explicit. Perception cannot be correct
until observation sources and occlusion rules are defined.

## Alternatives considered

### Store only engine transforms

Couples Core to presentation and loaded geometry. Rejected.

### One global floating-point coordinate system

Loses semantic containment and introduces floating drift and enormous-world
precision problems. Rejected.

### Store only semantic places

Sufficient for background simulation but insufficient for local hearing,
interaction distance, and presentation reconciliation. Rejected as the complete
model.

### Make all connections bidirectional

Cannot represent one-way passages, drops, escalators, or asymmetric permission.
Rejected.

### Add pathfinding immediately

Would prematurely choose dynamic traversal and cost semantics. Rejected.

### Put doors and visibility on connections

Mixes stable topology with changing gameplay state and perception policy.
Rejected.

## Attempt to disprove the decision

One parent per place cannot represent arbitrary overlapping semantic regions.
For example, a market may overlap a street district and a security zone. Those
should be modeled as separate membership components or query indexes rather than
turning containment into an ambiguous graph.

Local coordinates do not automatically transform between parent and child
places. This is deliberate. A room may use a different local origin or be
procedurally streamed. A future transform layer can add explicit versioned
mappings.

Connection distance alone cannot produce physically exact route length. It is a
stable declared edge metric. Presentation adapters or generated topology must
supply suitable values.

Millimeter integers do not represent sub-millimeter animation. Authoritative
social and gameplay simulation does not require that precision.

## Consequences

Positive:

- reusable semantic locations independent of presentation engines;
- exact deterministic local coordinates and radius checks;
- stable hierarchy for rooms, buildings, districts, and world cells;
- directed topology without hidden traversal policy;
- authoritative entity position as ordinary component data;
- deterministic construction and persistence;
- no floating geometry, physics, clocks, I/O, or providers.

Negative:

- no route planning yet;
- no dynamic doors or traversal permissions yet;
- no cross-place geometric distance;
- no overlapping containment memberships;
- adapters must map places and points to presentation transforms.

## Enforcement

Tests verify exact distance units, exact squared local geometry, same-place
position semantics, invalid hierarchy and connection rejection, construction
order determinism, containment, directed connection queries, authoritative
command and tick movement, and byte-identical mixed runtime continuation through
save/restore.

Repository verification enforces integer millimeters, bounded coordinates,
exact squared distance, immutable position components, cycle and endpoint
validation, deterministic sorting, read-only topology collections, and absence
of pathfinding policy, floating vectors, physics, presentation-engine coupling,
clocks, generated IDs, hidden execution, I/O, providers, and game-domain
vocabulary.
