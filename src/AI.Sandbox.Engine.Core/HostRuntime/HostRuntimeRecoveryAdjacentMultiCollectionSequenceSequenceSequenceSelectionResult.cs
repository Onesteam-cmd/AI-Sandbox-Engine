namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent multi-collection-sequence-sequence-sequence selection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelectionResult(
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus status,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<
            TRequest,
            TState,
            TCompletion>? selection)
    {
        Status = status;
        Summary = summary;
        Selection = selection;
    }

    /// <summary>Gets explicit adjacent selection outcome.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus Status { get; }

    /// <summary>Gets unchanged source summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets selection authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<
        TRequest,
        TState,
        TCompletion>? Selection { get; }

    /// <summary>Gets whether adjacent multi-collection-sequence-sequence-sequence was selected.</summary>
    public bool Succeeded =>
        Status is
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                .PreviousAdjacentMultiCollectionSequenceSequenceSequenceSelected or
            HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryStatus
                .NextAdjacentMultiCollectionSequenceSequenceSequenceSelected;
}
