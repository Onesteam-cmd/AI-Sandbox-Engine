namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery adjacent-window selection result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryAdjacentWindowSelectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentWindowSelectionResult(
        HostRuntimeRecoveryCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentWindowSelection<
            TRequest,
            TState,
            TCompletion>? selection)
    {
        Status = status;
        Summary = summary;
        Selection = selection;
    }

    /// <summary>Gets the explicit adjacent-window selection outcome.</summary>
    public HostRuntimeRecoveryCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets adjacent-window selection authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentWindowSelection<
        TRequest,
        TState,
        TCompletion>? Selection { get; }

    /// <summary>Gets whether adjacent-window authority was selected.</summary>
    public bool Succeeded =>
        Status is
            HostRuntimeRecoveryCheckpointRangeSummaryStatus
                .PreviousAdjacentWindowSelected or
            HostRuntimeRecoveryCheckpointRangeSummaryStatus
                .NextAdjacentWindowSelected;
}
