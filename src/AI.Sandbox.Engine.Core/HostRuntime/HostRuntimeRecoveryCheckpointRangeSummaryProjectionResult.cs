namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery checkpoint-range summary result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpointRangeSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCheckpointRangeSummaryProjectionResult(
        HostRuntimeRecoveryCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion> range,
        HostRuntimeRecoveryCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion>? summary)
    {
        Status = status;
        Range = range;
        Summary = summary;
    }

    /// <summary>Gets the explicit checkpoint-range summary outcome.</summary>
    public HostRuntimeRecoveryCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source checkpoint-range query authority.</summary>
    public HostRuntimeRecoveryCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion> Range { get; }

    /// <summary>Gets checkpoint-range summary authority when successful.</summary>
    public HostRuntimeRecoveryCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Summary { get; }

    /// <summary>Gets whether checkpoint-range summary authority was created.</summary>
    public bool Succeeded =>
        Status ==
        HostRuntimeRecoveryCheckpointRangeSummaryStatus
            .CheckpointRangeSummaryProjected;
}
