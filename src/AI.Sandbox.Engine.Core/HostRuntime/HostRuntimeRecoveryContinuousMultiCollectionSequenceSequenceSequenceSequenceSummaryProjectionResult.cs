namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery continuous multi-collection-sequence-sequence-sequence summary result.
/// </summary>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjectionResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryStatus status,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion> continuity,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Continuity = continuity;
        Summary = summary;
    }

    /// <summary>Gets the explicit multi-collection-sequence-summary outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryStatus Status { get; }

    /// <summary>Gets unchanged source continuity-validation authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets the created multi-collection summary, when successful.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether multi-collection-sequence-summary projection succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryStatus
            .ContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjected;
}
