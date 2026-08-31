namespace AI.Sandbox.Engine.Core.Perception;

/// <summary>
/// Represents a pure observed or ignored perception decision.
/// </summary>
/// <typeparam name="TSignal">The exact concrete signal type.</typeparam>
public sealed class PerceptionDecision<TSignal>
    where TSignal : notnull, IPerceptionSignal
{
    private PerceptionDecision(
        PerceptionDecisionStatus status,
        bool hasSignal,
        TSignal signal,
        PerceptionConfidence? confidence,
        string? ignoreReason)
    {
        Status = status;
        HasSignal = hasSignal;
        Signal = signal;
        Confidence = confidence;
        IgnoreReason = ignoreReason;
    }

    /// <summary>
    /// Gets the decision status.
    /// </summary>
    public PerceptionDecisionStatus Status { get; }

    /// <summary>
    /// Gets a value indicating whether this decision contains a signal.
    /// </summary>
    public bool HasSignal { get; }

    /// <summary>
    /// Gets the produced signal when observed.
    /// </summary>
    public TSignal Signal { get; }

    /// <summary>
    /// Gets the signal confidence when observed.
    /// </summary>
    public PerceptionConfidence? Confidence { get; }

    /// <summary>
    /// Gets the internal reason when ignored.
    /// </summary>
    public string? IgnoreReason { get; }

    /// <summary>
    /// Produces one subjective signal for the observer.
    /// </summary>
    /// <param name="signal">The immutable concrete signal.</param>
    /// <param name="confidence">
    /// The initialized non-zero subjective confidence.
    /// </param>
    /// <returns>An observed decision.</returns>
    public static PerceptionDecision<TSignal> Observe(
        TSignal signal,
        PerceptionConfidence confidence)
    {
        PerceptionTypePolicy.EnsureConcrete<TSignal>(
            nameof(TSignal));
        PerceptionTypePolicy.EnsureValue(
            signal,
            nameof(signal));
        confidence.EnsureUsableForObservation();

        return new PerceptionDecision<TSignal>(
            PerceptionDecisionStatus.Observed,
            true,
            signal,
            confidence,
            null);
    }

    /// <summary>
    /// Ignores the candidate stimulus for this observer.
    /// </summary>
    /// <param name="reason">The non-empty internal reason.</param>
    /// <returns>An ignored decision.</returns>
    public static PerceptionDecision<TSignal> Ignore(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        PerceptionTypePolicy.EnsureConcrete<TSignal>(
            nameof(TSignal));

        return new PerceptionDecision<TSignal>(
            PerceptionDecisionStatus.Ignored,
            false,
            default!,
            null,
            reason);
    }
}
