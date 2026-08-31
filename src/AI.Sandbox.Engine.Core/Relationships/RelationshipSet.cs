using AI.Sandbox.Engine.Core.Components;
using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Relationships;

/// <summary>
/// Stores one owner's current directed relationships for one exact payload type.
/// </summary>
/// <typeparam name="TState">The exact immutable relationship-state type.</typeparam>
public sealed class RelationshipSet<TState> : IComponent
    where TState : notnull, IRelationshipState
{
    private readonly RelationshipEntry<TState>[] entries;
    private readonly IReadOnlyList<RelationshipEntry<TState>> readOnlyEntries;

    private RelationshipSet(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId,
        RelationshipEntry<TState>[] entries)
    {
        WorldId = worldId;
        OwnerEntityId = ownerEntityId;
        this.entries = (RelationshipEntry<TState>[])entries.Clone();
        readOnlyEntries = Array.AsReadOnly(this.entries);
    }

    /// <summary>
    /// Gets the world to which this relationship set belongs.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the entity whose subjective relationships are represented.
    /// </summary>
    public Id<EntityIdKind> OwnerEntityId { get; }

    /// <summary>
    /// Gets current relationships ordered by stable target identity.
    /// </summary>
    public IReadOnlyList<RelationshipEntry<TState>> Entries =>
        readOnlyEntries;

    /// <summary>
    /// Gets the number of current directed relationships.
    /// </summary>
    public int Count => entries.Length;

    /// <summary>
    /// Gets a value indicating whether no current relationship exists.
    /// </summary>
    public bool IsEmpty => entries.Length == 0;

    /// <summary>
    /// Creates an empty relationship set for one owner and world.
    /// </summary>
    /// <param name="worldId">The world identity.</param>
    /// <param name="ownerEntityId">The relationship owner identity.</param>
    /// <returns>The empty immutable relationship set.</returns>
    public static RelationshipSet<TState> Create(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId)
    {
        EnsureNonEmpty(worldId, nameof(worldId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        RelationshipTypePolicy.EnsureConcrete<TState>(nameof(TState));

        return new RelationshipSet<TState>(
            worldId,
            ownerEntityId,
            []);
    }

    /// <summary>
    /// Restores and deterministically orders current relationship entries.
    /// </summary>
    /// <param name="worldId">The world identity.</param>
    /// <param name="ownerEntityId">The relationship owner identity.</param>
    /// <param name="entries">The persisted current entries.</param>
    /// <returns>The validated immutable relationship set.</returns>
    public static RelationshipSet<TState> Restore(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId,
        IEnumerable<RelationshipEntry<TState>> entries)
    {
        EnsureNonEmpty(worldId, nameof(worldId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        RelationshipTypePolicy.EnsureConcrete<TState>(nameof(TState));
        ArgumentNullException.ThrowIfNull(entries);

        var materialized = entries.ToArray();
        foreach (var entry in materialized)
        {
            ArgumentNullException.ThrowIfNull(entry);

            if (entry.TargetEntityId == ownerEntityId)
            {
                throw new ArgumentException(
                    "A directed relationship target must differ from its owner.",
                    nameof(entries));
            }

            if (entry.LastChange.WorldId != worldId)
            {
                throw new ArgumentException(
                    "A relationship entry belongs to another world.",
                    nameof(entries));
            }

            if (entry.LastChange.OwnerEntityId != ownerEntityId)
            {
                throw new ArgumentException(
                    "A relationship entry belongs to another owner.",
                    nameof(entries));
            }

            if (entry.LastChange.TargetEntityId != entry.TargetEntityId)
            {
                throw new ArgumentException(
                    "A relationship entry has inconsistent target provenance.",
                    nameof(entries));
            }
        }

        Array.Sort(
            materialized,
            static (left, right) =>
                left.TargetEntityId.CompareTo(right.TargetEntityId));

        for (var index = 1; index < materialized.Length; index++)
        {
            if (materialized[index - 1].TargetEntityId ==
                materialized[index].TargetEntityId)
            {
                throw new ArgumentException(
                    "Relationship targets must be unique within one exact " +
                    "relationship set.",
                    nameof(entries));
            }
        }

        return new RelationshipSet<TState>(
            worldId,
            ownerEntityId,
            materialized);
    }

    /// <summary>
    /// Attempts to read the current relationship toward one target.
    /// </summary>
    /// <param name="targetEntityId">The directed target identity.</param>
    /// <param name="entry">The current entry when found.</param>
    /// <returns><see langword="true"/> when the target exists.</returns>
    public bool TryGet(
        Id<EntityIdKind> targetEntityId,
        out RelationshipEntry<TState>? entry)
    {
        EnsureTarget(targetEntityId);

        var index = FindIndex(targetEntityId);
        if (index < 0)
        {
            entry = null;
            return false;
        }

        entry = entries[index];
        return true;
    }

    /// <summary>
    /// Adds one new directed current relationship.
    /// </summary>
    /// <param name="targetEntityId">The directed target identity.</param>
    /// <param name="state">The exact immutable current payload.</param>
    /// <param name="change">The explicit latest-change provenance.</param>
    /// <returns>The explicit immutable mutation result.</returns>
    public RelationshipMutationResult<TState> Add(
        Id<EntityIdKind> targetEntityId,
        TState state,
        RelationshipChangeReference change)
    {
        EnsureTarget(targetEntityId);
        RelationshipTypePolicy.EnsureValue(state, nameof(state));
        ArgumentNullException.ThrowIfNull(change);

        var changeStatus = GetChangeStatus(targetEntityId, change);
        if (changeStatus.HasValue)
        {
            return Result(
                changeStatus.Value,
                this,
                null,
                change);
        }

        var index = FindIndex(targetEntityId);
        if (index >= 0)
        {
            return Result(
                RelationshipMutationStatus.RelationshipAlreadyExists,
                this,
                entries[index],
                change);
        }

        var entry = RelationshipEntry<TState>.Create(
            targetEntityId,
            state,
            change);
        var insertionIndex = ~index;
        var next = new RelationshipEntry<TState>[entries.Length + 1];

        Array.Copy(entries, 0, next, 0, insertionIndex);
        next[insertionIndex] = entry;
        Array.Copy(
            entries,
            insertionIndex,
            next,
            insertionIndex + 1,
            entries.Length - insertionIndex);

        return Result(
            RelationshipMutationStatus.Added,
            new RelationshipSet<TState>(
                WorldId,
                OwnerEntityId,
                next),
            entry,
            change);
    }

    /// <summary>
    /// Revises one existing directed current relationship.
    /// </summary>
    /// <param name="targetEntityId">The directed target identity.</param>
    /// <param name="expectedRevision">The exact expected current revision.</param>
    /// <param name="state">The replacement immutable current payload.</param>
    /// <param name="change">The explicit latest-change provenance.</param>
    /// <returns>The explicit immutable mutation result.</returns>
    public RelationshipMutationResult<TState> Revise(
        Id<EntityIdKind> targetEntityId,
        uint expectedRevision,
        TState state,
        RelationshipChangeReference change)
    {
        EnsureTarget(targetEntityId);
        RelationshipTypePolicy.EnsureValue(state, nameof(state));
        ArgumentNullException.ThrowIfNull(change);

        var index = FindIndex(targetEntityId);
        if (index < 0)
        {
            return Result(
                RelationshipMutationStatus.RelationshipNotFound,
                this,
                null,
                change);
        }

        var current = entries[index];
        if (current.Revision != expectedRevision)
        {
            return Result(
                RelationshipMutationStatus.RevisionConflict,
                this,
                current,
                change);
        }

        var changeStatus = GetChangeStatus(targetEntityId, change);
        if (changeStatus.HasValue)
        {
            return Result(
                changeStatus.Value,
                this,
                current,
                change);
        }

        if (change.WorldStateVersion.Value <
                current.LastUpdatedWorldStateVersion.Value ||
            change.SimulationTick <
                current.LastUpdatedSimulationTick)
        {
            return Result(
                RelationshipMutationStatus.TemporalRegression,
                this,
                current,
                change);
        }

        if (EqualityComparer<TState>.Default.Equals(current.State, state) &&
            current.LastChange == change)
        {
            return Result(
                RelationshipMutationStatus.Unchanged,
                this,
                current,
                change);
        }

        var revised = current.Update(state, change);
        var next = (RelationshipEntry<TState>[])entries.Clone();
        next[index] = revised;

        return Result(
            RelationshipMutationStatus.Revised,
            new RelationshipSet<TState>(
                WorldId,
                OwnerEntityId,
                next),
            revised,
            change);
    }

    /// <summary>
    /// Removes one existing directed current relationship.
    /// </summary>
    /// <param name="targetEntityId">The directed target identity.</param>
    /// <param name="expectedRevision">The exact expected current revision.</param>
    /// <param name="change">The explicit removal provenance.</param>
    /// <returns>The explicit immutable mutation result.</returns>
    public RelationshipMutationResult<TState> Remove(
        Id<EntityIdKind> targetEntityId,
        uint expectedRevision,
        RelationshipChangeReference change)
    {
        EnsureTarget(targetEntityId);
        ArgumentNullException.ThrowIfNull(change);

        var index = FindIndex(targetEntityId);
        if (index < 0)
        {
            return Result(
                RelationshipMutationStatus.RelationshipNotFound,
                this,
                null,
                change);
        }

        var current = entries[index];
        if (current.Revision != expectedRevision)
        {
            return Result(
                RelationshipMutationStatus.RevisionConflict,
                this,
                current,
                change);
        }

        var changeStatus = GetChangeStatus(targetEntityId, change);
        if (changeStatus.HasValue)
        {
            return Result(
                changeStatus.Value,
                this,
                current,
                change);
        }

        if (change.WorldStateVersion.Value <
                current.LastUpdatedWorldStateVersion.Value ||
            change.SimulationTick <
                current.LastUpdatedSimulationTick)
        {
            return Result(
                RelationshipMutationStatus.TemporalRegression,
                this,
                current,
                change);
        }

        var next = new RelationshipEntry<TState>[entries.Length - 1];
        Array.Copy(entries, 0, next, 0, index);
        Array.Copy(
            entries,
            index + 1,
            next,
            index,
            entries.Length - index - 1);

        return Result(
            RelationshipMutationStatus.Removed,
            new RelationshipSet<TState>(
                WorldId,
                OwnerEntityId,
                next),
            current,
            change);
    }

    private RelationshipMutationStatus? GetChangeStatus(
        Id<EntityIdKind> targetEntityId,
        RelationshipChangeReference change)
    {
        if (change.WorldId != WorldId)
        {
            return RelationshipMutationStatus.ChangeWorldMismatch;
        }

        if (change.OwnerEntityId != OwnerEntityId)
        {
            return RelationshipMutationStatus.ChangeOwnerMismatch;
        }

        if (change.TargetEntityId != targetEntityId)
        {
            return RelationshipMutationStatus.ChangeTargetMismatch;
        }

        return null;
    }

    private int FindIndex(Id<EntityIdKind> targetEntityId)
    {
        var lower = 0;
        var upper = entries.Length - 1;

        while (lower <= upper)
        {
            var middle = lower + ((upper - lower) / 2);
            var comparison =
                entries[middle].TargetEntityId.CompareTo(targetEntityId);

            if (comparison == 0)
            {
                return middle;
            }

            if (comparison < 0)
            {
                lower = middle + 1;
            }
            else
            {
                upper = middle - 1;
            }
        }

        return ~lower;
    }

    private void EnsureTarget(Id<EntityIdKind> targetEntityId)
    {
        EnsureNonEmpty(targetEntityId, nameof(targetEntityId));

        if (targetEntityId == OwnerEntityId)
        {
            throw new ArgumentException(
                "A directed relationship target must differ from its owner.",
                nameof(targetEntityId));
        }
    }

    private static RelationshipMutationResult<TState> Result(
        RelationshipMutationStatus status,
        RelationshipSet<TState> relationshipSet,
        RelationshipEntry<TState>? entry,
        RelationshipChangeReference change)
    {
        return new RelationshipMutationResult<TState>(
            status,
            relationshipSet,
            entry,
            change);
    }

    private static void EnsureNonEmpty<TKind>(
        Id<TKind> id,
        string parameterName)
        where TKind : struct
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Relationship identifiers cannot be empty.",
                parameterName);
        }
    }
}
