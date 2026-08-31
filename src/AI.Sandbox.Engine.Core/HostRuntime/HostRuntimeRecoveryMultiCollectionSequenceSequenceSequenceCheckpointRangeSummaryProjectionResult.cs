namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable range-summary projection result.</summary>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjectionResult(
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion> range,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Range = range;
        Summary = summary;
    }

    /// <summary>Gets explicit range-summary projection outcome.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source range authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion> Range { get; }

    /// <summary>Gets summary authority when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether summary authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryStatus
            .MultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjected;
}
