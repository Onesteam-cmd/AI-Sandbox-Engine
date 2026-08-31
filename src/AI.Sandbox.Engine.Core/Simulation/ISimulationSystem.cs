using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Simulation;

/// <summary>
/// Evaluates one deterministic, side-effect-free stage of a logical simulation
/// tick.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
/// <remarks>
/// Systems execute sequentially in registration order. Implementations must not
/// mutate the supplied state, dispatch events, perform external I/O, read
/// wall-clock time, or rely on hidden global randomness.
/// </remarks>
public interface ISimulationSystem<TState>
    where TState : class, IWorldState
{
    /// <summary>
    /// Evaluates this system once for the current logical tick.
    /// </summary>
    /// <param name="context">
    /// The immutable system context and current working state.
    /// </param>
    /// <returns>An unchanged, updated, or rejected decision.</returns>
    public SimulationSystemDecision<TState> Execute(
        SimulationSystemContext<TState> context);
}
