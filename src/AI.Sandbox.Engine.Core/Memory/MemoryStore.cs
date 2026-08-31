using AI.Sandbox.Engine.Core.Components;
using AI.Sandbox.Engine.Core.Entities;
using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Memory;

/// <summary>
/// Provides this memory-model API member.
/// </summary>
public sealed class MemoryStore<TContent> : IComponent
    where TContent : notnull, IMemoryContent
{
    private readonly IReadOnlyList<MemoryEntry<TContent>> entries;
    private readonly Dictionary<
        Id<MemoryIdKind>,
        MemoryEntry<TContent>> entryById;

    private MemoryStore(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId,
        MemoryEntry<TContent>[] entries)
    {
        WorldId = worldId;
        OwnerEntityId = ownerEntityId;

        var copy =
            (MemoryEntry<TContent>[])entries.Clone();
        this.entries = Array.AsReadOnly(copy);
        entryById = copy.ToDictionary(
            entry => entry.MemoryId);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<EntityIdKind> OwnerEntityId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public int Count => entries.Count;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public bool IsEmpty => entries.Count == 0;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public IReadOnlyList<MemoryEntry<TContent>> Entries => entries;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryStore<TContent> Create(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId)
    {
        EnsureNonEmpty(worldId, nameof(worldId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        MemoryTypePolicy.EnsureConcrete<TContent>(nameof(TContent));

        return new MemoryStore<TContent>(
            worldId,
            ownerEntityId,
            []);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryStore<TContent> Restore(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId,
        IEnumerable<MemoryEntry<TContent>> entries)
    {
        EnsureNonEmpty(worldId, nameof(worldId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        MemoryTypePolicy.EnsureConcrete<TContent>(nameof(TContent));
        ArgumentNullException.ThrowIfNull(entries);

        var materialized = entries
            .OrderBy(entry => entry.MemoryId)
            .ToArray();
        var seen = new HashSet<Id<MemoryIdKind>>();

        foreach (var entry in materialized)
        {
            ArgumentNullException.ThrowIfNull(entry);

            if (!seen.Add(entry.MemoryId))
            {
                throw new ArgumentException(
                    $"Memory '{entry.MemoryId}' appears more than once.",
                    nameof(entries));
            }

            ValidateOriginOrThrow(
                worldId,
                ownerEntityId,
                entry.Origin);
        }

        return new MemoryStore<TContent>(
            worldId,
            ownerEntityId,
            materialized);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public bool TryGet(
        Id<MemoryIdKind> memoryId,
        out MemoryEntry<TContent>? entry)
    {
        if (memoryId.IsEmpty)
        {
            entry = null;
            return false;
        }

        return entryById.TryGetValue(
            memoryId,
            out entry);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryMutationResult<TContent> Encode(
        Id<MemoryIdKind> memoryId,
        TContent content,
        MemoryOriginReference origin,
        MemoryStrength strength,
        MemorySalience salience)
    {
        if (memoryId.IsEmpty)
        {
            throw new ArgumentException(
                "A memory ID cannot be empty.",
                nameof(memoryId));
        }

        MemoryTypePolicy.EnsureValue(content, nameof(content));
        ArgumentNullException.ThrowIfNull(origin);
        strength.EnsureUsableForRetainedMemory();
        salience.EnsureInitialized();

        if (entryById.TryGetValue(
            memoryId,
            out var duplicate))
        {
            return Result(
                MemoryMutationStatus.MemoryAlreadyExists,
                this,
                duplicate);
        }

        var originStatus = GetOriginStatus(origin);
        if (originStatus is { } status)
        {
            return Result(status, this, null);
        }

        var entry = MemoryEntry<TContent>.Create(
            memoryId,
            content,
            origin,
            strength,
            salience);

        return Result(
            MemoryMutationStatus.Encoded,
            WithAdded(entry),
            entry);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryMutationResult<TContent> Reinforce(
        Id<MemoryIdKind> memoryId,
        uint expectedRevision,
        ushort strengthIncrease,
        ushort salienceIncrease,
        WorldStateVersion worldStateVersion,
        ulong simulationTick)
    {
        var validation = ValidateUpdate(
            memoryId,
            expectedRevision,
            worldStateVersion,
            simulationTick);

        if (validation.Status is { } status)
        {
            return Result(
                status,
                this,
                validation.Entry);
        }

        var current = validation.Entry!;
        var strength = current.Strength.Increase(
            strengthIncrease);
        var salience = current.Salience.Increase(
            salienceIncrease);

        if (strength == current.Strength &&
            salience == current.Salience)
        {
            return Result(
                MemoryMutationStatus.Unchanged,
                this,
                current);
        }

        var revised = current.Update(
            strength,
            salience,
            worldStateVersion,
            simulationTick);

        return Result(
            MemoryMutationStatus.Reinforced,
            WithReplaced(revised),
            revised);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryMutationResult<TContent> Weaken(
        Id<MemoryIdKind> memoryId,
        uint expectedRevision,
        ushort strengthDecrease,
        ushort salienceDecrease,
        WorldStateVersion worldStateVersion,
        ulong simulationTick)
    {
        var validation = ValidateUpdate(
            memoryId,
            expectedRevision,
            worldStateVersion,
            simulationTick);

        if (validation.Status is { } status)
        {
            return Result(
                status,
                this,
                validation.Entry);
        }

        var current = validation.Entry!;
        var strength = current.Strength.Decrease(
            strengthDecrease);
        var salience = current.Salience.Decrease(
            salienceDecrease);

        if (strength.IsZero)
        {
            return Result(
                MemoryMutationStatus.Forgotten,
                Without(memoryId),
                current);
        }

        if (strength == current.Strength &&
            salience == current.Salience)
        {
            return Result(
                MemoryMutationStatus.Unchanged,
                this,
                current);
        }

        var revised = current.Update(
            strength,
            salience,
            worldStateVersion,
            simulationTick);

        return Result(
            MemoryMutationStatus.Weakened,
            WithReplaced(revised),
            revised);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryMutationResult<TContent> Remove(
        Id<MemoryIdKind> memoryId,
        uint expectedRevision)
    {
        ValidateMemoryIdAndRevision(
            memoryId,
            expectedRevision);

        if (!entryById.TryGetValue(
            memoryId,
            out var current))
        {
            return Result(
                MemoryMutationStatus.MemoryNotFound,
                this,
                null);
        }

        if (current.Revision != expectedRevision)
        {
            return Result(
                MemoryMutationStatus.RevisionConflict,
                this,
                current);
        }

        return Result(
            MemoryMutationStatus.Removed,
            Without(memoryId),
            current);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryRecallResult<TContent> Recall(
        MemoryRecallQuery query)
    {
        if (query.MaximumResults == 0 ||
            query.MinimumStrength.IsEmpty ||
            query.MinimumSalience.IsEmpty)
        {
            throw new ArgumentException(
                "The default memory recall query is invalid.",
                nameof(query));
        }

        var recalled = entries
            .Where(entry =>
                entry.Strength.BasisPoints >=
                    query.MinimumStrength.BasisPoints &&
                entry.Salience.BasisPoints >=
                    query.MinimumSalience.BasisPoints)
            .OrderByDescending(entry => entry.RecallPriority)
            .ThenByDescending(entry => entry.Strength.BasisPoints)
            .ThenByDescending(entry => entry.Salience.BasisPoints)
            .ThenByDescending(
                entry => entry.LastUpdatedWorldStateVersion.Value)
            .ThenByDescending(entry => entry.LastUpdatedSimulationTick)
            .ThenBy(entry => entry.MemoryId)
            .Take(query.MaximumResults)
            .ToArray();

        return new MemoryRecallResult<TContent>(
            query,
            recalled);
    }

    private (
        MemoryMutationStatus? Status,
        MemoryEntry<TContent>? Entry) ValidateUpdate(
            Id<MemoryIdKind> memoryId,
            uint expectedRevision,
            WorldStateVersion worldStateVersion,
            ulong simulationTick)
    {
        ValidateMemoryIdAndRevision(
            memoryId,
            expectedRevision);

        if (!entryById.TryGetValue(
            memoryId,
            out var current))
        {
            return (
                MemoryMutationStatus.MemoryNotFound,
                null);
        }

        if (current.Revision != expectedRevision)
        {
            return (
                MemoryMutationStatus.RevisionConflict,
                current);
        }

        if (worldStateVersion.Value <
                current.LastUpdatedWorldStateVersion.Value ||
            simulationTick <
                current.LastUpdatedSimulationTick)
        {
            return (
                MemoryMutationStatus.TemporalRegression,
                current);
        }

        return (null, current);
    }

    private static void ValidateMemoryIdAndRevision(
        Id<MemoryIdKind> memoryId,
        uint expectedRevision)
    {
        if (memoryId.IsEmpty)
        {
            throw new ArgumentException(
                "A memory ID cannot be empty.",
                nameof(memoryId));
        }

        if (expectedRevision == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedRevision),
                expectedRevision,
                "An expected memory revision must be positive.");
        }
    }

    private MemoryStore<TContent> WithAdded(
        MemoryEntry<TContent> entry)
    {
        var updated = entries
            .Append(entry)
            .OrderBy(item => item.MemoryId)
            .ToArray();

        return new MemoryStore<TContent>(
            WorldId,
            OwnerEntityId,
            updated);
    }

    private MemoryStore<TContent> WithReplaced(
        MemoryEntry<TContent> entry)
    {
        var updated = entries
            .Select(current =>
                current.MemoryId == entry.MemoryId
                    ? entry
                    : current)
            .OrderBy(item => item.MemoryId)
            .ToArray();

        return new MemoryStore<TContent>(
            WorldId,
            OwnerEntityId,
            updated);
    }

    private MemoryStore<TContent> Without(
        Id<MemoryIdKind> memoryId)
    {
        return new MemoryStore<TContent>(
            WorldId,
            OwnerEntityId,
            entries
                .Where(entry => entry.MemoryId != memoryId)
                .ToArray());
    }

    private MemoryMutationStatus? GetOriginStatus(
        MemoryOriginReference origin)
    {
        if (origin.WorldId != WorldId)
        {
            return MemoryMutationStatus.OriginWorldMismatch;
        }

        if (origin.OwnerEntityId != OwnerEntityId)
        {
            return MemoryMutationStatus.OriginOwnerMismatch;
        }

        return null;
    }

    private static void ValidateOriginOrThrow(
        Id<WorldIdKind> worldId,
        Id<EntityIdKind> ownerEntityId,
        MemoryOriginReference origin)
    {
        ArgumentNullException.ThrowIfNull(origin);

        if (origin.WorldId != worldId)
        {
            throw new ArgumentException(
                "Memory origin belongs to a different world.",
                nameof(origin));
        }

        if (origin.OwnerEntityId != ownerEntityId)
        {
            throw new ArgumentException(
                "Memory origin belongs to a different owner.",
                nameof(origin));
        }
    }

    private static MemoryMutationResult<TContent> Result(
        MemoryMutationStatus status,
        MemoryStore<TContent> memoryStore,
        MemoryEntry<TContent>? entry)
    {
        return new MemoryMutationResult<TContent>(
            status,
            memoryStore,
            entry);
    }

    private static void EnsureNonEmpty<TKind>(
        Id<TKind> id,
        string parameterName)
        where TKind : struct
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Memory store IDs cannot be empty.",
                parameterName);
        }
    }
}
