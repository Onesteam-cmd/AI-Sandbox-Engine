namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable multi-collection-sequence-sequence checkpoint-range continuity result.
/// </summary>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidationResult(
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus status,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<
            TRequest,
            TState,
            TCompletion> adjacentMultiCollectionSequence,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        Summary = summary;
        AdjacentMultiCollectionSequence = adjacentMultiCollectionSequence;
        Validation = validation;
    }

    /// <summary>Gets explicit continuity-validation outcome.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus Status { get; }

    /// <summary>Gets unchanged multi-collection-sequence range-summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent multi-collection-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentMultiCollectionSequence { get; }

    /// <summary>Gets continuity authority when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether exact continuity was validated.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
            .MultiCollectionSequenceSequenceCheckpointRangeContinuityValidated;
}
