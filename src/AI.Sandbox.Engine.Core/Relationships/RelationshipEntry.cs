using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Relationships;

/// <summary>
/// Represents the current directed relationship state held by one owner toward
/// one target.
/// </summary>
/// <typeparam name="TState">The exact immutable relationship-state type.</typeparam>
public sealed class RelationshipEntry<TState>
    where TState : notnull, IRelationshipState
{
    private RelationshipEntry(
        Id<EntityIdKind> targetEntityId,
        uint revision,
        TState state,
        RelationshipChangeReference lastChange,
        WorldStateVersion establishedWorldStateVersion,
        ulong establishedSimulationTick,
        WorldStateVersion lastUpdatedWorldStateVersion,
        ulong lastUpdatedSimulationTick)
    {
        TargetEntityId = targetEntityId;
        Revision = revision;
        State = state;
        LastChange = lastChange;
        EstablishedWorldStateVersion = establishedWorldStateVersion;
        EstablishedSimulationTick = establishedSimulationTick;
        LastUpdatedWorldStateVersion = lastUpdatedWorldStateVersion;
        LastUpdatedSimulationTick = lastUpdatedSimulationTick;
    }

    /// <summary>
    /// Gets the entity toward which this relationship is directed.
    /// </summary>
    public Id<EntityIdKind> TargetEntityId { get; }

    /// <summary>
    /// Gets the positive optimistic revision of the current state.
    /// </summary>
    public uint Revision { get; }

    /// <summary>
    /// Gets the exact immutable current relationship payload.
    /// </summary>
    public TState State { get; }

    /// <summary>
    /// Gets the compact provenance of the latest successful change.
    /// </summary>
    public RelationshipChangeReference LastChange { get; }

    /// <summary>
    /// Gets the World State version at which the relationship was established.
    /// </summary>
    public WorldStateVersion EstablishedWorldStateVersion { get; }

    /// <summary>
    /// Gets the logical tick at which the relationship was established.
    /// </summary>
    public ulong EstablishedSimulationTick { get; }

    /// <summary>
    /// Gets the World State version observed for the latest update.
    /// </summary>
    public WorldStateVersion LastUpdatedWorldStateVersion { get; }

    /// <summary>
    /// Gets the logical tick observed for the latest update.
    /// </summary>
    public ulong LastUpdatedSimulationTick { get; }

    /// <summary>
    /// Restores a validated current relationship entry from persistence.
    /// </summary>
    /// <param name="targetEntityId">The directed target identity.</param>
    /// <param name="revision">The positive current revision.</param>
    /// <param name="state">The exact immutable relationship payload.</param>
    /// <param name="lastChange">The latest compact change provenance.</param>
    /// <param name="establishedWorldStateVersion">The establishment version.</param>
    /// <param name="establishedSimulationTick">The establishment tick.</param>
    /// <param name="lastUpdatedWorldStateVersion">The latest update version.</param>
    /// <param name="lastUpdatedSimulationTick">The latest update tick.</param>
    /// <returns>The validated immutable relationship entry.</returns>
    public static RelationshipEntry<TState> Restore(
        Id<EntityIdKind> targetEntityId,
        uint revision,
        TState state,
        RelationshipChangeReference lastChange,
        WorldStateVersion establishedWorldStateVersion,
        ulong establishedSimulationTick,
        WorldStateVersion lastUpdatedWorldStateVersion,
        ulong lastUpdatedSimulationTick)
    {
        if (targetEntityId.IsEmpty)
        {
            throw new ArgumentException(
                "A relationship target ID cannot be empty.",
                nameof(targetEntityId));
        }

        if (revision == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(revision),
                revision,
                "A relationship revision must be positive.");
        }

        RelationshipTypePolicy.EnsureConcrete<TState>(nameof(TState));
        RelationshipTypePolicy.EnsureValue(state, nameof(state));
        ArgumentNullException.ThrowIfNull(lastChange);

        if (lastChange.TargetEntityId != targetEntityId)
        {
            throw new ArgumentException(
                "The latest change target does not match the relationship entry.",
                nameof(lastChange));
        }

        if (establishedWorldStateVersion.Value >
                lastUpdatedWorldStateVersion.Value ||
            establishedSimulationTick >
                lastUpdatedSimulationTick)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lastUpdatedWorldStateVersion),
                lastUpdatedWorldStateVersion,
                "The latest update cannot precede relationship establishment.");
        }

        if (lastChange.WorldStateVersion !=
                lastUpdatedWorldStateVersion ||
            lastChange.SimulationTick !=
                lastUpdatedSimulationTick)
        {
            throw new ArgumentException(
                "The latest change metadata must equal the latest update " +
                "metadata.",
                nameof(lastChange));
        }

        return new RelationshipEntry<TState>(
            targetEntityId,
            revision,
            state,
            lastChange,
            establishedWorldStateVersion,
            establishedSimulationTick,
            lastUpdatedWorldStateVersion,
            lastUpdatedSimulationTick);
    }

    internal static RelationshipEntry<TState> Create(
        Id<EntityIdKind> targetEntityId,
        TState state,
        RelationshipChangeReference change)
    {
        return Restore(
            targetEntityId,
            revision: 1,
            state,
            change,
            change.WorldStateVersion,
            change.SimulationTick,
            change.WorldStateVersion,
            change.SimulationTick);
    }

    internal RelationshipEntry<TState> Update(
        TState state,
        RelationshipChangeReference change)
    {
        return Restore(
            TargetEntityId,
            checked(Revision + 1),
            state,
            change,
            EstablishedWorldStateVersion,
            EstablishedSimulationTick,
            change.WorldStateVersion,
            change.SimulationTick);
    }
}
