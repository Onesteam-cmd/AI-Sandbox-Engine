using AI.Sandbox.Engine.Core.Identifiers;
using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Simulation;

internal sealed class SimulationSystemRegistration<TState>
    where TState : class, IWorldState
{
    public SimulationSystemRegistration(
        Id<SimulationSystemIdKind> systemId,
        ISimulationSystem<TState> system)
    {
        SystemId = systemId;
        System = system;
    }

    public Id<SimulationSystemIdKind> SystemId { get; }

    public ISimulationSystem<TState> System { get; }
}
