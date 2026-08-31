using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Memory;

/// <summary>
/// Provides this memory-model API member.
/// </summary>
public sealed class MemoryEntry<TContent>
    where TContent : notnull, IMemoryContent
{
    private MemoryEntry(
        Id<MemoryIdKind> memoryId,
        uint revision,
        TContent content,
        MemoryOriginReference origin,
        MemoryStrength strength,
        MemorySalience salience,
        WorldStateVersion encodedWorldStateVersion,
        ulong encodedSimulationTick,
        WorldStateVersion lastUpdatedWorldStateVersion,
        ulong lastUpdatedSimulationTick)
    {
        MemoryId = memoryId;
        Revision = revision;
        Content = content;
        Origin = origin;
        Strength = strength;
        Salience = salience;
        EncodedWorldStateVersion = encodedWorldStateVersion;
        EncodedSimulationTick = encodedSimulationTick;
        LastUpdatedWorldStateVersion = lastUpdatedWorldStateVersion;
        LastUpdatedSimulationTick = lastUpdatedSimulationTick;
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public Id<MemoryIdKind> MemoryId { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public uint Revision { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public TContent Content { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryOriginReference Origin { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryStrength Strength { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemorySalience Salience { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public WorldStateVersion EncodedWorldStateVersion { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public ulong EncodedSimulationTick { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public WorldStateVersion LastUpdatedWorldStateVersion { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public ulong LastUpdatedSimulationTick { get; }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public uint RecallPriority =>
        (uint)Strength.BasisPoints +
        Salience.BasisPoints;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryEntry<TContent> Restore(
        Id<MemoryIdKind> memoryId,
        uint revision,
        TContent content,
        MemoryOriginReference origin,
        MemoryStrength strength,
        MemorySalience salience,
        WorldStateVersion encodedWorldStateVersion,
        ulong encodedSimulationTick,
        WorldStateVersion lastUpdatedWorldStateVersion,
        ulong lastUpdatedSimulationTick)
    {
        if (memoryId.IsEmpty)
        {
            throw new ArgumentException(
                "A memory ID cannot be empty.",
                nameof(memoryId));
        }

        if (revision == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(revision),
                revision,
                "A memory revision must be positive.");
        }

        MemoryTypePolicy.EnsureConcrete<TContent>(nameof(TContent));
        MemoryTypePolicy.EnsureValue(content, nameof(content));
        ArgumentNullException.ThrowIfNull(origin);
        strength.EnsureUsableForRetainedMemory();
        salience.EnsureInitialized();

        if (origin.WorldStateVersion.Value >
            encodedWorldStateVersion.Value ||
            origin.SimulationTick >
            encodedSimulationTick)
        {
            throw new ArgumentOutOfRangeException(
                nameof(encodedWorldStateVersion),
                encodedWorldStateVersion,
                "Encoding cannot precede memory origin.");
        }

        if (encodedWorldStateVersion.Value >
            lastUpdatedWorldStateVersion.Value ||
            encodedSimulationTick >
            lastUpdatedSimulationTick)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lastUpdatedWorldStateVersion),
                lastUpdatedWorldStateVersion,
                "Last update cannot precede encoding.");
        }

        return new MemoryEntry<TContent>(
            memoryId,
            revision,
            content,
            origin,
            strength,
            salience,
            encodedWorldStateVersion,
            encodedSimulationTick,
            lastUpdatedWorldStateVersion,
            lastUpdatedSimulationTick);
    }

    internal static MemoryEntry<TContent> Create(
        Id<MemoryIdKind> memoryId,
        TContent content,
        MemoryOriginReference origin,
        MemoryStrength strength,
        MemorySalience salience)
    {
        return Restore(
            memoryId,
            revision: 1,
            content,
            origin,
            strength,
            salience,
            origin.WorldStateVersion,
            origin.SimulationTick,
            origin.WorldStateVersion,
            origin.SimulationTick);
    }

    internal MemoryEntry<TContent> Update(
        MemoryStrength strength,
        MemorySalience salience,
        WorldStateVersion worldStateVersion,
        ulong simulationTick)
    {
        return Restore(
            MemoryId,
            checked(Revision + 1),
            Content,
            Origin,
            strength,
            salience,
            EncodedWorldStateVersion,
            EncodedSimulationTick,
            worldStateVersion,
            simulationTick);
    }
}
