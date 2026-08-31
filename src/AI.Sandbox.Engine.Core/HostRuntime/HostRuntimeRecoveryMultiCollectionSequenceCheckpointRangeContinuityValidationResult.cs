namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable multi-collection-sequence checkpoint-range continuity result.
/// </summary>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidationResult(
        HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus status,
        HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentMultiCollectionProjection<
            TRequest,
            TState,
            TCompletion> adjacentMultiCollection,
        HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        Summary = summary;
        AdjacentMultiCollection = adjacentMultiCollection;
        Validation = validation;
    }

    /// <summary>Gets explicit continuity-validation outcome.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus Status { get; }

    /// <summary>Gets unchanged multi-collection range-summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent multi-collection authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionProjection<
        TRequest,
        TState,
        TCompletion> AdjacentMultiCollection { get; }

    /// <summary>Gets continuity authority when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether exact continuity was validated.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
            .MultiCollectionSequenceCheckpointRangeContinuityValidated;
}
