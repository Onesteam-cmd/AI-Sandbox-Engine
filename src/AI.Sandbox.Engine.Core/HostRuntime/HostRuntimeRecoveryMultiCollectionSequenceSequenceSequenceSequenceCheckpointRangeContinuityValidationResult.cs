namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable multi-collection-sequence-sequence-sequence-sequence checkpoint-range continuity result.
/// </summary>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidationResult(
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus status,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<
            TRequest,
            TState,
            TCompletion> adjacentMultiCollectionSequenceSequenceSequence,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        Summary = summary;
        AdjacentMultiCollectionSequenceSequenceSequence = adjacentMultiCollectionSequenceSequenceSequence;
        Validation = validation;
    }

    /// <summary>Gets explicit continuity-validation outcome.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus Status { get; }

    /// <summary>Gets unchanged multi-collection-sequence range-summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent multi-collection-sequence-sequence-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentMultiCollectionSequenceSequenceSequence { get; }

    /// <summary>Gets continuity authority when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether exact continuity was validated.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
            .MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeContinuityValidated;
}
