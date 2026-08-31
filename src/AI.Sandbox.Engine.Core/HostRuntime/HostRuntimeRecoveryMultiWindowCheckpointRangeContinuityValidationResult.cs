namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable multi-window checkpoint-range continuity result.
/// </summary>
public sealed record HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidationResult(
        HostRuntimeRecoveryAdjacentSequenceProjectionStatus status,
        HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentSequenceProjection<
            TRequest,
            TState,
            TCompletion> adjacentSequence,
        HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        Summary = summary;
        AdjacentSequence = adjacentSequence;
        Validation = validation;
    }

    /// <summary>Gets explicit multi-window continuity-validation outcome.</summary>
    public HostRuntimeRecoveryAdjacentSequenceProjectionStatus Status { get; }

    /// <summary>Gets unchanged multi-window checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent-sequence authority.</summary>
    public HostRuntimeRecoveryAdjacentSequenceProjection<
        TRequest,
        TState,
        TCompletion> AdjacentSequence { get; }

    /// <summary>Gets continuity authority when successful.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether exact continuity was validated.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentSequenceProjectionStatus
            .MultiWindowCheckpointRangeContinuityValidated;
}
