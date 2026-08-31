namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable multi-sequence checkpoint-range continuity result.
/// </summary>
public sealed record HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidationResult(
        HostRuntimeRecoveryAdjacentCollectionProjectionStatus status,
        HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentCollectionProjection<
            TRequest,
            TState,
            TCompletion> adjacentCollection,
        HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        Summary = summary;
        AdjacentCollection = adjacentCollection;
        Validation = validation;
    }

    /// <summary>Gets explicit continuity-validation outcome.</summary>
    public HostRuntimeRecoveryAdjacentCollectionProjectionStatus Status { get; }

    /// <summary>Gets unchanged multi-sequence range-summary authority.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent-collection authority.</summary>
    public HostRuntimeRecoveryAdjacentCollectionProjection<
        TRequest,
        TState,
        TCompletion> AdjacentCollection { get; }

    /// <summary>Gets continuity authority when successful.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether exact continuity was validated.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentCollectionProjectionStatus
            .MultiSequenceCheckpointRangeContinuityValidated;
}
