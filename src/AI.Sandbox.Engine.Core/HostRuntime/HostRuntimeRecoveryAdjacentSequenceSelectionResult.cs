namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent-sequence selection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentSequenceSelectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentSequenceSelectionResult(
        HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentSequenceSelection<
            TRequest,
            TState,
            TCompletion>? selection)
    {
        Status = status;
        Summary = summary;
        Selection = selection;
    }

    /// <summary>Gets explicit adjacent-sequence selection outcome.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source summary authority.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets selection authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentSequenceSelection<
        TRequest,
        TState,
        TCompletion>? Selection { get; }

    /// <summary>Gets whether adjacent sequence was selected.</summary>
    public bool Succeeded =>
        Status is
            HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                .PreviousAdjacentSequenceSelected or
            HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryStatus
                .NextAdjacentSequenceSelected;
}
