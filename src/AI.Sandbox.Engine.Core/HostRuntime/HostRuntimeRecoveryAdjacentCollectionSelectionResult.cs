namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent-collection selection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentCollectionSelectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentCollectionSelectionResult(
        HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentCollectionSelection<
            TRequest,
            TState,
            TCompletion>? selection)
    {
        Status = status;
        Summary = summary;
        Selection = selection;
    }

    /// <summary>Gets explicit adjacent-collection selection outcome.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source summary authority.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets selection authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSelection<
        TRequest,
        TState,
        TCompletion>? Selection { get; }

    /// <summary>Gets whether adjacent collection was selected.</summary>
    public bool Succeeded =>
        Status is
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                .PreviousAdjacentCollectionSelected or
            HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryStatus
                .NextAdjacentCollectionSelected;
}
