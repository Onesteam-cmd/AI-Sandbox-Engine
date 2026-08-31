using AI.Sandbox.Engine.Core.WorldState;

namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Reports one attempted perception evaluation without mutating World State.
/// </summary>
/// <typeparam name="TState">The immutable World State root type.</typeparam>
/// <typeparam name="TSignal">The exact concrete signal type.</typeparam>
public sealed class PerceptionEvaluationResult<TState, TSignal>
    where TState : class, IWorldState
    where TSignal : notnull, IPerceptionSignal
{
    private PerceptionEvaluationResult(
        PerceptionEvaluationStatus status,
        WorldStateSnapshot<TState> snapshot,
        bool evaluatorWasExecuted,
        PerceptionObservation<TSignal>? observation,
        string? failureReason)
    {
        Status = status;
        Snapshot = snapshot;
        EvaluatorWasExecuted = evaluatorWasExecuted;
        Observation = observation;
        FailureReason = failureReason;
    }

    /// <summary>
    /// Gets the evaluation outcome.
    /// </summary>
    public PerceptionEvaluationStatus Status { get; }

    /// <summary>
    /// Gets the authoritative snapshot associated with the result.
    /// </summary>
    public WorldStateSnapshot<TState> Snapshot { get; }

    /// <summary>
    /// Gets a value indicating whether the evaluator ran.
    /// </summary>
    public bool EvaluatorWasExecuted { get; }

    /// <summary>
    /// Gets the subjective observation only when observed.
    /// </summary>
    public PerceptionObservation<TSignal>? Observation { get; }

    /// <summary>
    /// Gets the internal diagnostic reason when no observation was returned.
    /// </summary>
    public string? FailureReason { get; }

    /// <summary>
    /// Gets a value indicating whether a subjective signal was produced.
    /// </summary>
    public bool WasObserved =>
        Status == PerceptionEvaluationStatus.Observed;

    internal static PerceptionEvaluationResult<TState, TSignal> Observed(
        WorldStateSnapshot<TState> snapshot,
        PerceptionObservation<TSignal> observation)
    {
        return new PerceptionEvaluationResult<TState, TSignal>(
            PerceptionEvaluationStatus.Observed,
            snapshot,
            true,
            observation,
            null);
    }

    internal static PerceptionEvaluationResult<TState, TSignal> Failed(
        PerceptionEvaluationStatus status,
        WorldStateSnapshot<TState> snapshot,
        bool evaluatorWasExecuted,
        string failureReason)
    {
        if (status == PerceptionEvaluationStatus.Observed)
        {
            throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "A failed perception result cannot use Observed status.");
        }

        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentException.ThrowIfNullOrWhiteSpace(failureReason);

        return new PerceptionEvaluationResult<TState, TSignal>(
            status,
            snapshot,
            evaluatorWasExecuted,
            null,
            failureReason);
    }
}
