namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-sequence range-summary result.</summary>
public sealed record HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjectionResult(
        HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion> range,
        HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Range = range;
        Summary = summary;
    }

    /// <summary>Gets explicit multi-sequence summary outcome.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source range authority.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion> Range { get; }

    /// <summary>Gets summary authority when successful.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether summary authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
            .MultiSequenceCheckpointRangeSummaryProjected;
}
