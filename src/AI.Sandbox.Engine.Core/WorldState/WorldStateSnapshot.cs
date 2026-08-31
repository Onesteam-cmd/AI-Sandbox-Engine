using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.WorldState;

/// <summary>
/// Represents one immutable, versioned observation of authoritative world
/// state.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
public sealed class WorldStateSnapshot<TState>
    where TState : class, IWorldState
{
    internal WorldStateSnapshot(
        Id<WorldIdKind> worldId,
        WorldStateVersion version,
        ulong simulationTick,
        TState state)
    {
        if (worldId.IsEmpty)
        {
            throw new ArgumentException(
                "A world-state snapshot requires a non-empty world identifier.",
                nameof(worldId));
        }

        ArgumentNullException.ThrowIfNull(state);

        WorldId = worldId;
        Version = version;
        SimulationTick = simulationTick;
        State = state;
    }

    /// <summary>
    /// Gets the world identifier.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the authoritative state version.
    /// </summary>
    public WorldStateVersion Version { get; }

    /// <summary>
    /// Gets the logical simulation tick represented by this state.
    /// </summary>
    public ulong SimulationTick { get; }

    /// <summary>
    /// Gets the immutable world-state root.
    /// </summary>
    public TState State { get; }
}
