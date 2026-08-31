using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Version-gates and evaluates exact candidate stimuli without mutating
/// authoritative World State.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
/// <remarks>
/// The processor contains no queue, retry, event dispatch, persistence, memory,
/// knowledge, randomness, clocks, I/O, or provider calls.
/// </remarks>
public sealed class PerceptionProcessor<TState>
    where TState : class, IWorldState
{
    private readonly WorldStateManager<TState> worldStateManager;
    private readonly IReadOnlyDictionary<
        PerceptionEvaluatorKey,
        object> evaluators;

    internal PerceptionProcessor(
        WorldStateManager<TState> worldStateManager,
        Dictionary<PerceptionEvaluatorKey, object> evaluators)
    {
        this.worldStateManager = worldStateManager;
        this.evaluators =
            new System.Collections.ObjectModel.ReadOnlyDictionary<
                PerceptionEvaluatorKey,
                object>(evaluators);
    }

    /// <summary>
    /// Gets the number of exact evaluator pairs.
    /// </summary>
    public int EvaluatorCount => evaluators.Count;

    /// <summary>
    /// Evaluates one exact candidate stimulus against one immutable snapshot.
    /// </summary>
    /// <typeparam name="TStimulus">The exact stimulus type.</typeparam>
    /// <typeparam name="TSignal">The exact signal type.</typeparam>
    /// <param name="envelope">The version-gated candidate stimulus.</param>
    /// <returns>An observed, ignored, missing, or stale result.</returns>
    public PerceptionEvaluationResult<TState, TSignal>
        Evaluate<TStimulus, TSignal>(
            PerceptionStimulusEnvelope<TStimulus> envelope)
        where TStimulus : notnull, IPerceptionStimulus
        where TSignal : notnull, IPerceptionSignal
    {
        ArgumentNullException.ThrowIfNull(envelope);
        PerceptionTypePolicy.EnsureConcrete<TStimulus>(
            nameof(TStimulus));
        PerceptionTypePolicy.EnsureConcrete<TSignal>(
            nameof(TSignal));

        var observedSnapshot = worldStateManager.Read();

        if (envelope.WorldId != observedSnapshot.WorldId)
        {
            return PerceptionEvaluationResult<TState, TSignal>.Failed(
                PerceptionEvaluationStatus.WorldMismatch,
                observedSnapshot,
                false,
                "The candidate stimulus targets a different world.");
        }

        if (envelope.ExpectedWorldStateVersion !=
            observedSnapshot.Version)
        {
            return PerceptionEvaluationResult<TState, TSignal>.Failed(
                PerceptionEvaluationStatus.VersionConflict,
                observedSnapshot,
                false,
                "The candidate stimulus was formed from a stale version.");
        }

        if (envelope.ExpectedSimulationTick !=
            observedSnapshot.SimulationTick)
        {
            return PerceptionEvaluationResult<TState, TSignal>.Failed(
                PerceptionEvaluationStatus.SimulationTickMismatch,
                observedSnapshot,
                false,
                "The candidate stimulus was formed at a different tick.");
        }

        var key = new PerceptionEvaluatorKey(
            typeof(TStimulus),
            typeof(TSignal));

        if (!evaluators.TryGetValue(
            key,
            out var untypedEvaluator))
        {
            return PerceptionEvaluationResult<TState, TSignal>.Failed(
                PerceptionEvaluationStatus.EvaluatorNotRegistered,
                observedSnapshot,
                false,
                $"No evaluator is registered for stimulus " +
                $"'{typeof(TStimulus)}' and signal '{typeof(TSignal)}'.");
        }

        var evaluator =
            (IPerceptionEvaluator<TState, TStimulus, TSignal>)
                untypedEvaluator;
        var context =
            new PerceptionContext<TState, TStimulus>(
                observedSnapshot,
                envelope);
        var decision = evaluator.Evaluate(context);

        if (decision is null)
        {
            throw new InvalidOperationException(
                $"Perception evaluator for stimulus '{typeof(TStimulus)}' " +
                $"and signal '{typeof(TSignal)}' returned null.");
        }

        var currentSnapshot = worldStateManager.Read();

        if (currentSnapshot.Version != observedSnapshot.Version ||
            currentSnapshot.SimulationTick !=
                observedSnapshot.SimulationTick)
        {
            return PerceptionEvaluationResult<TState, TSignal>.Failed(
                PerceptionEvaluationStatus.VersionConflict,
                currentSnapshot,
                true,
                "Authoritative World State changed during evaluation.");
        }

        return decision.Status switch
        {
            PerceptionDecisionStatus.Observed =>
                CreateObservedResult<TStimulus, TSignal>(
                    envelope,
                    observedSnapshot,
                    decision),

            PerceptionDecisionStatus.Ignored =>
                CreateIgnoredResult<TStimulus, TSignal>(
                    observedSnapshot,
                    decision),

            _ => throw CreateInconsistentDecisionException<
                TStimulus,
                TSignal>(),
        };
    }

    private static PerceptionEvaluationResult<TState, TSignal>
        CreateObservedResult<TStimulus, TSignal>(
            PerceptionStimulusEnvelope<TStimulus> envelope,
            WorldStateSnapshot<TState> snapshot,
            PerceptionDecision<TSignal> decision)
        where TStimulus : notnull, IPerceptionStimulus
        where TSignal : notnull, IPerceptionSignal
    {
        if (!decision.HasSignal ||
            decision.Confidence is not { } confidence ||
            decision.IgnoreReason is not null)
        {
            throw CreateInconsistentDecisionException<
                TStimulus,
                TSignal>();
        }

        confidence.EnsureUsableForObservation();

        var observation = new PerceptionObservation<TSignal>(
            envelope.StimulusId,
            envelope.ChannelId,
            envelope.ObserverEntityId,
            snapshot.WorldId,
            snapshot.Version,
            snapshot.SimulationTick,
            confidence,
            decision.Signal);

        return PerceptionEvaluationResult<TState, TSignal>.Observed(
            snapshot,
            observation);
    }

    private static PerceptionEvaluationResult<TState, TSignal>
        CreateIgnoredResult<TStimulus, TSignal>(
            WorldStateSnapshot<TState> snapshot,
            PerceptionDecision<TSignal> decision)
        where TStimulus : notnull, IPerceptionStimulus
        where TSignal : notnull, IPerceptionSignal
    {
        if (decision.HasSignal ||
            decision.Confidence is not null ||
            string.IsNullOrWhiteSpace(decision.IgnoreReason))
        {
            throw CreateInconsistentDecisionException<
                TStimulus,
                TSignal>();
        }

        return PerceptionEvaluationResult<TState, TSignal>.Failed(
            PerceptionEvaluationStatus.Ignored,
            snapshot,
            true,
            decision.IgnoreReason);
    }

    private static InvalidOperationException
        CreateInconsistentDecisionException<TStimulus, TSignal>()
        where TStimulus : notnull, IPerceptionStimulus
        where TSignal : notnull, IPerceptionSignal
    {
        return new InvalidOperationException(
            $"Perception evaluator for stimulus '{typeof(TStimulus)}' and " +
            $"signal '{typeof(TSignal)}' returned an internally " +
            "inconsistent decision.");
    }
}
