using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Simulation;

/// <summary>
/// Provides immutable context to one simulation system during one logical tick.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
public sealed class SimulationSystemContext<TState>
    where TState : class, IWorldState
{
    internal SimulationSystemContext(
        Id<WorldIdKind> worldId,
        WorldStateVersion startingVersion,
        ulong previousSimulationTick,
        ulong targetSimulationTick,
        Id<SimulationSystemIdKind> systemId,
        int systemIndex,
        TState state)
    {
        WorldId = worldId;
        StartingVersion = startingVersion;
        PreviousSimulationTick = previousSimulationTick;
        TargetSimulationTick = targetSimulationTick;
        SystemId = systemId;
        SystemIndex = systemIndex;
        State = state;
    }

    /// <summary>
    /// Gets the world being simulated.
    /// </summary>
    public Id<WorldIdKind> WorldId { get; }

    /// <summary>
    /// Gets the authoritative version observed at the start of the tick.
    /// </summary>
    public WorldStateVersion StartingVersion { get; }

    /// <summary>
    /// Gets the previously committed logical simulation tick.
    /// </summary>
    public ulong PreviousSimulationTick { get; }

    /// <summary>
    /// Gets the logical tick being evaluated.
    /// </summary>
    public ulong TargetSimulationTick { get; }

    /// <summary>
    /// Gets the stable identifier of the executing simulation system.
    /// </summary>
    public Id<SimulationSystemIdKind> SystemId { get; }

    /// <summary>
    /// Gets the zero-based deterministic registration index of the system.
    /// </summary>
    public int SystemIndex { get; }

    /// <summary>
    /// Gets the immutable working state produced by all earlier systems in this
    /// tick.
    /// </summary>
    public TState State { get; }
}
