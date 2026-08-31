namespace AI.Sandbox.Engine.Core.WorldState;

/// <summary>
/// Evaluates one proposed transition from a captured world-state snapshot.
/// </summary>
/// <typeparam name="TState">The immutable world-state root type.</typeparam>
/// <remarks>
/// Implementations must not mutate the supplied snapshot or perform hidden
/// retries. Evaluation may be executed outside the manager's commit lock, so a
/// successful decision can still lose an optimistic concurrency race.
/// </remarks>
public interface IWorldStateTransition<TState>
    where TState : class, IWorldState
{
    /// <summary>
    /// Evaluates the transition against the captured state.
    /// </summary>
    /// <param name="current">The state snapshot observed for evaluation.</param>
    /// <returns>An accepted next state or an explicit rejection.</returns>
    public WorldStateTransitionDecision<TState> Evaluate(
        WorldStateSnapshot<TState> current);
}
