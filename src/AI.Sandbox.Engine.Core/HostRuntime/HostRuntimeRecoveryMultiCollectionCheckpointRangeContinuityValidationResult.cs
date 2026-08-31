namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable multi-collection checkpoint-range continuity result.
/// </summary>
public sealed record HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidationResult(
        HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus status,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentCollectionSequenceProjection<
            TRequest,
            TState,
            TCompletion> adjacentCollectionSequence,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        Summary = summary;
        AdjacentCollectionSequence = adjacentCollectionSequence;
        Validation = validation;
    }

    /// <summary>Gets explicit continuity-validation outcome.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus Status { get; }

    /// <summary>Gets unchanged multi-collection range-summary authority.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent collection-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentCollectionSequence { get; }

    /// <summary>Gets continuity authority when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether exact continuity was validated.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
            .MultiCollectionCheckpointRangeContinuityValidated;
}
