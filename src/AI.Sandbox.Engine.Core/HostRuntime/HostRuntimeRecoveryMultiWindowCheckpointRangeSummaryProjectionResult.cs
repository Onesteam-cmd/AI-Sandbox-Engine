namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-window range-summary result.</summary>
public sealed record HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjectionResult(
        HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion> range,
        HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Range = range;
        Summary = summary;
    }

    /// <summary>Gets explicit multi-window summary outcome.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source range authority.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion> Range { get; }

    /// <summary>Gets summary authority when successful.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether summary authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
            .MultiWindowCheckpointRangeSummaryProjected;
}
