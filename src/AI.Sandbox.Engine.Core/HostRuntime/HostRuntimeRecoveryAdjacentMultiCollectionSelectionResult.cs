namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent multi-collection selection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentMultiCollectionSelectionResult(
        HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentMultiCollectionSelection<
            TRequest,
            TState,
            TCompletion>? selection)
    {
        Status = status;
        Summary = summary;
        Selection = selection;
    }

    /// <summary>Gets explicit adjacent multi-collection selection outcome.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets selection authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSelection<
        TRequest,
        TState,
        TCompletion>? Selection { get; }

    /// <summary>Gets whether adjacent multi-collection was selected.</summary>
    public bool Succeeded =>
        Status is
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                .PreviousAdjacentMultiCollectionSelected or
            HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryStatus
                .NextAdjacentMultiCollectionSelected;
}
