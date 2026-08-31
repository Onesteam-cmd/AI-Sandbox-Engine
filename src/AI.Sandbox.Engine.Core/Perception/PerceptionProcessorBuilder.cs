using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Registers one pure evaluator for each exact stimulus/signal type pair before
/// creating a perception processor.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
public sealed class PerceptionProcessorBuilder<TState>
    where TState : class, IWorldState
{
    private readonly Dictionary<
        PerceptionEvaluatorKey,
        object> evaluators = [];
    private bool isBuilt;

    /// <summary>
    /// Registers one evaluator for one exact concrete stimulus/signal pair.
    /// </summary>
    /// <typeparam name="TStimulus">The exact stimulus type.</typeparam>
    /// <typeparam name="TSignal">The exact signal type.</typeparam>
    /// <param name="evaluator">The pure evaluator.</param>
    /// <returns>This builder.</returns>
    public PerceptionProcessorBuilder<TState> Add<TStimulus, TSignal>(
        IPerceptionEvaluator<TState, TStimulus, TSignal> evaluator)
        where TStimulus : notnull, IPerceptionStimulus
        where TSignal : notnull, IPerceptionSignal
    {
        ThrowIfBuilt();
        PerceptionTypePolicy.EnsureConcrete<TStimulus>(
            nameof(TStimulus));
        PerceptionTypePolicy.EnsureConcrete<TSignal>(
            nameof(TSignal));
        ArgumentNullException.ThrowIfNull(evaluator);

        var key = new PerceptionEvaluatorKey(
            typeof(TStimulus),
            typeof(TSignal));

        if (!evaluators.TryAdd(key, evaluator))
        {
            throw new ArgumentException(
                $"An evaluator for stimulus '{typeof(TStimulus)}' and " +
                $"signal '{typeof(TSignal)}' is already registered.",
                nameof(evaluator));
        }

        return this;
    }

    /// <summary>
    /// Creates a read-only perception processor bound to one authoritative
    /// World State Manager and permanently consumes this builder.
    /// </summary>
    /// <param name="worldStateManager">The authority to observe.</param>
    /// <returns>The exact-type perception processor.</returns>
    public PerceptionProcessor<TState> Build(
        WorldStateManager<TState> worldStateManager)
    {
        ThrowIfBuilt();
        ArgumentNullException.ThrowIfNull(worldStateManager);
        isBuilt = true;

        return new PerceptionProcessor<TState>(
            worldStateManager,
            new Dictionary<PerceptionEvaluatorKey, object>(
                evaluators));
    }

    private void ThrowIfBuilt()
    {
        if (isBuilt)
        {
            throw new InvalidOperationException(
                "A perception processor builder cannot be reused after Build.");
        }
    }
}
