namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-collection range-summary result.</summary>
public sealed record HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjectionResult(
        HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion> range,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Range = range;
        Summary = summary;
    }

    /// <summary>Gets explicit multi-collection summary outcome.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source range authority.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion> Range { get; }

    /// <summary>Gets summary authority when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether summary authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
            .MultiCollectionCheckpointRangeSummaryProjected;
}
