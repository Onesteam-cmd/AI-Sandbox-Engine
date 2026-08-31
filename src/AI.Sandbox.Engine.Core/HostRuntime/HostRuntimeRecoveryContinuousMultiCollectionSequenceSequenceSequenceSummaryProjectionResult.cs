namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery continuous multi-collection-sequence-sequence summary result.
/// </summary>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjectionResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus status,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion> continuity,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Continuity = continuity;
        Summary = summary;
    }

    /// <summary>Gets the explicit multi-collection-sequence-summary outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus Status { get; }

    /// <summary>Gets unchanged source continuity-validation authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets the created multi-collection summary, when successful.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether multi-collection-sequence-summary projection succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
            .ContinuousMultiCollectionSequenceSequenceSequenceSummaryProjected;
}
