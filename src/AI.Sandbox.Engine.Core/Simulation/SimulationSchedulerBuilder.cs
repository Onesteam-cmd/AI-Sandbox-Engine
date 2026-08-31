using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Simulation;

/// <summary>
/// Defines the fixed deterministic order of simulation systems before creating
/// a scheduler.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
public sealed class SimulationSchedulerBuilder<TState>
    where TState : class, IWorldState
{
    private readonly List<SimulationSystemRegistration<TState>> registrations =
        [];
    private readonly HashSet<Id<SimulationSystemIdKind>> registeredIds = [];
    private bool isBuilt;

    /// <summary>
    /// Registers one simulation system at the end of the deterministic system
    /// sequence.
    /// </summary>
    /// <param name="systemId">
    /// The stable non-empty identifier used for diagnostics and replay metadata.
    /// </param>
    /// <param name="system">The simulation system instance.</param>
    /// <returns>This builder.</returns>
    public SimulationSchedulerBuilder<TState> Add(
        Id<SimulationSystemIdKind> systemId,
        ISimulationSystem<TState> system)
    {
        ThrowIfBuilt();

        if (systemId.IsEmpty)
        {
            throw new ArgumentException(
                "A simulation system identifier cannot be empty.",
                nameof(systemId));
        }

        ArgumentNullException.ThrowIfNull(system);

        if (!registeredIds.Add(systemId))
        {
            throw new ArgumentException(
                "Simulation system identifiers must be unique.",
                nameof(systemId));
        }

        registrations.Add(
            new SimulationSystemRegistration<TState>(systemId, system));

        return this;
    }

    /// <summary>
    /// Creates a scheduler bound to one authoritative World State Manager and
    /// permanently consumes this builder.
    /// </summary>
    /// <param name="worldStateManager">
    /// The sole authoritative manager advanced by this scheduler.
    /// </param>
    /// <returns>A deterministic simulation scheduler.</returns>
    public SimulationScheduler<TState> Build(
        WorldStateManager<TState> worldStateManager)
    {
        ThrowIfBuilt();
        ArgumentNullException.ThrowIfNull(worldStateManager);
        isBuilt = true;

        return new SimulationScheduler<TState>(
            worldStateManager,
            registrations.ToArray());
    }

    private void ThrowIfBuilt()
    {
        if (isBuilt)
        {
            throw new InvalidOperationException(
                "A simulation scheduler builder cannot be reused after Build.");
        }
    }
}
