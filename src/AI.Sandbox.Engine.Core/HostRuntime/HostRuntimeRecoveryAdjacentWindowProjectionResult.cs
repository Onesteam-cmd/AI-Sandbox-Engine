namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery adjacent-window projection result.
/// </summary>
public sealed record HostRuntimeRecoveryAdjacentWindowProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentWindowProjectionResult(
        HostRuntimeRecoveryAdjacentWindowProjectionStatus status,
        HostRuntimeRecoveryAdjacentWindowSelection<
            TRequest,
            TState,
            TCompletion> selection,
        HostRuntimeRecoveryAdjacentWindowProjection<
            TRequest,
            TState,
            TCompletion>? projection)
    {
        Status = status;
        Selection = selection;
        Projection = projection;
    }

    /// <summary>Gets the explicit adjacent-window projection outcome.</summary>
    public HostRuntimeRecoveryAdjacentWindowProjectionStatus Status { get; }

    /// <summary>Gets unchanged adjacent-window selection authority.</summary>
    public HostRuntimeRecoveryAdjacentWindowSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets projected adjacent-window authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentWindowProjection<
        TRequest,
        TState,
        TCompletion>? Projection { get; }

    /// <summary>Gets whether adjacent-window authority was projected.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentWindowProjectionStatus
            .AdjacentWindowProjected;
}
