namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent-sequence projection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentSequenceProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentSequenceProjectionResult(
        HostRuntimeRecoveryAdjacentSequenceProjectionStatus status,
        HostRuntimeRecoveryAdjacentSequenceSelection<
            TRequest,
            TState,
            TCompletion> selection,
        HostRuntimeRecoveryAdjacentSequenceProjection<
            TRequest,
            TState,
            TCompletion>? projection)
    {
        Status = status;
        Selection = selection;
        Projection = projection;
    }

    /// <summary>Gets explicit adjacent-sequence projection outcome.</summary>
    public HostRuntimeRecoveryAdjacentSequenceProjectionStatus Status { get; }

    /// <summary>Gets unchanged adjacent-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets projected adjacent-sequence authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentSequenceProjection<
        TRequest,
        TState,
        TCompletion>? Projection { get; }

    /// <summary>Gets whether adjacent-sequence authority was projected.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentSequenceProjectionStatus
            .AdjacentSequenceProjected;
}
