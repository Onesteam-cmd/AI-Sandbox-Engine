namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery continuous collection-pair summary result.
/// </summary>
public sealed record HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousCollectionPairSummaryProjectionResult(
        HostRuntimeRecoveryContinuousCollectionPairStatus status,
        HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion> continuity,
        HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Continuity = continuity;
        Summary = summary;
    }

    /// <summary>Gets the explicit collection-pair-summary outcome.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairStatus Status { get; }

    /// <summary>Gets unchanged source continuity-validation authority.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets the created collection-pair summary, when successful.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether collection-pair-summary projection succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousCollectionPairStatus
            .ContinuousCollectionPairSummaryProjected;
}
