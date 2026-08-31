using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Purely evaluates one exact candidate stimulus type into one exact subjective
/// signal type for one observer.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
/// <typeparam name="TStimulus">The exact concrete stimulus type.</typeparam>
/// <typeparam name="TSignal">The exact concrete signal type.</typeparam>
/// <remarks>
/// Evaluators execute at most once per processor call. They must not mutate
/// World State, dispatch events, persist data, read wall-clock time, generate
/// IDs, call providers, or write memory or knowledge.
///
/// An evaluator's signal is not automatically a fact, memory, belief, or item
/// of knowledge.
/// </remarks>
public interface IPerceptionEvaluator<TState, TStimulus, TSignal>
    where TState : class, IWorldState
    where TStimulus : notnull, IPerceptionStimulus
    where TSignal : notnull, IPerceptionSignal
{
    /// <summary>
    /// Evaluates one candidate stimulus against one immutable snapshot.
    /// </summary>
    /// <param name="context">The observer, stimulus, and snapshot context.</param>
    /// <returns>An observed or ignored pure decision.</returns>
    public PerceptionDecision<TSignal> Evaluate(
        PerceptionContext<TState, TStimulus> context);
}
