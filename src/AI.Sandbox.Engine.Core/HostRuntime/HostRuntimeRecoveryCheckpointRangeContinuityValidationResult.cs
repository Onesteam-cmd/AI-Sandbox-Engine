namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery checkpoint-range continuity result.
/// </summary>
public sealed record HostRuntimeRecoveryCheckpointRangeContinuityValidationResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCheckpointRangeContinuityValidationResult(
        HostRuntimeRecoveryAdjacentWindowProjectionStatus status,
        HostRuntimeRecoveryCheckpointRangeSummaryProjection<
            TRequest,
            TState,
            TCompletion> summary,
        HostRuntimeRecoveryAdjacentWindowProjection<
            TRequest,
            TState,
            TCompletion> adjacentWindow,
        HostRuntimeRecoveryCheckpointRangeContinuityValidation<
            TRequest,
            TState,
            TCompletion>? validation)
    {
        Status = status;
        Summary = summary;
        AdjacentWindow = adjacentWindow;
        Validation = validation;
    }

    /// <summary>Gets the explicit checkpoint-range continuity outcome.</summary>
    public HostRuntimeRecoveryAdjacentWindowProjectionStatus Status { get; }

    /// <summary>Gets unchanged checkpoint-range summary authority.</summary>
    public HostRuntimeRecoveryCheckpointRangeSummaryProjection<
        TRequest,
        TState,
        TCompletion> Summary { get; }

    /// <summary>Gets unchanged projected adjacent-window authority.</summary>
    public HostRuntimeRecoveryAdjacentWindowProjection<
        TRequest,
        TState,
        TCompletion> AdjacentWindow { get; }

    /// <summary>Gets continuity-validation authority when successful.</summary>
    public HostRuntimeRecoveryCheckpointRangeContinuityValidation<
        TRequest,
        TState,
        TCompletion>? Validation { get; }

    /// <summary>Gets whether exact checkpoint-range continuity was validated.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentWindowProjectionStatus
            .CheckpointRangeContinuityValidated;
}
