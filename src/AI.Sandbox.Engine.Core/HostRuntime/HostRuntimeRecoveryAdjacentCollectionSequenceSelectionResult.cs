namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent collection-sequence selection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentCollectionSequenceSelectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentCollectionSequenceSelectionResult(
        HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentCollectionSequenceSelection<
            TRequest,
            TState,
            TCompletion>? selection)
    {
        Status = status;
        Summary = summary;
        Selection = selection;
    }

    /// <summary>Gets explicit adjacent collection-sequence selection outcome.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets selection authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSequenceSelection<
        TRequest,
        TState,
        TCompletion>? Selection { get; }

    /// <summary>Gets whether an adjacent collection sequence was selected.</summary>
    public bool Succeeded =>
        Status is
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                .PreviousAdjacentCollectionSequenceSelected or
            HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryStatus
                .NextAdjacentCollectionSequenceSelected;
}
