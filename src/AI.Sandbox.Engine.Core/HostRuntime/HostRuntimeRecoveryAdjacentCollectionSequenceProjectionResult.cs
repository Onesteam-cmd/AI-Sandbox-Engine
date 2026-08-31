namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent collection-sequence projection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentCollectionSequenceProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentCollectionSequenceProjectionResult(
        HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus status,
        HostRuntimeRecoveryAdjacentCollectionSequenceSelection<
            TRequest,
            TState,
            TCompletion> selection,
        HostRuntimeRecoveryAdjacentCollectionSequenceProjection<
            TRequest,
            TState,
            TCompletion>? projection)
    {
        Status = status;
        Selection = selection;
        Projection = projection;
    }

    /// <summary>Gets explicit adjacent collection-sequence projection outcome.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus Status { get; }

    /// <summary>Gets unchanged adjacent collection-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets projected adjacent collection-sequence authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSequenceProjection<
        TRequest,
        TState,
        TCompletion>? Projection { get; }

    /// <summary>Gets whether adjacent collection-sequence authority was projected.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentCollectionSequenceProjectionStatus
            .AdjacentCollectionSequenceProjected;
}
