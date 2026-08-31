namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-collection-sequence range-summary result.</summary>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjectionResult(
        HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion> range,
        HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Range = range;
        Summary = summary;
    }

    /// <summary>Gets explicit multi-collection-sequence summary outcome.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source range authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion> Range { get; }

    /// <summary>Gets summary authority when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether summary authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
            .MultiCollectionSequenceCheckpointRangeSummaryProjected;
}
