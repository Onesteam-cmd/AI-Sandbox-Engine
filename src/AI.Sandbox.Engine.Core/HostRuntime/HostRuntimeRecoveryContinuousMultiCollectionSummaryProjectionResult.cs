namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery continuous multi-collection summary result.
/// </summary>
public sealed record HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryContinuousMultiCollectionSummaryProjectionResult(
        HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus status,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion> continuity,
        HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Continuity = continuity;
        Summary = summary;
    }

    /// <summary>Gets the explicit multi-collection-summary outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus Status { get; }

    /// <summary>Gets unchanged source continuity-validation authority.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion> Continuity { get; }

    /// <summary>Gets the created multi-collection summary, when successful.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether multi-collection-summary projection succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
            .ContinuousMultiCollectionSummaryProjected;
}
