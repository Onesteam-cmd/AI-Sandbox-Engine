namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable multi-collection-sequence-sequence-sequence checkpoint-range continuity result.
/// </summary>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidationResult(
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus status,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<
            TRequest,
            TState,
            TCompletion> adjacentMultiCollectionSequenceSequence,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        Summary = summary;
        AdjacentMultiCollectionSequenceSequence = adjacentMultiCollectionSequenceSequence;
        Validation = validation;
    }

    /// <summary>Gets explicit continuity-validation outcome.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus Status { get; }

    /// <summary>Gets unchanged multi-collection-sequence range-summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent multi-collection-sequence-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentMultiCollectionSequenceSequence { get; }

    /// <summary>Gets continuity authority when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether exact continuity was validated.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
            .MultiCollectionSequenceSequenceSequenceCheckpointRangeContinuityValidated;
}
