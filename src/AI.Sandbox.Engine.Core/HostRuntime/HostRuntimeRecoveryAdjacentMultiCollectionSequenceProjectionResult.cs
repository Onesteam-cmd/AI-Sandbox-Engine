namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent multi-collection-sequence projection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionResult(
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus status,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<
            TRequest,
            TState,
            TCompletion> selection,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<
            TRequest,
            TState,
            TCompletion>? projection)
    {
        Status = status;
        Selection = selection;
        Projection = projection;
    }

    /// <summary>Gets explicit adjacent multi-collection-sequence projection outcome.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus Status { get; }

    /// <summary>Gets unchanged adjacent multi-collection-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets projected adjacent multi-collection-sequence authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjection<
        TRequest,
        TState,
        TCompletion>? Projection { get; }

    /// <summary>Gets whether adjacent multi-collection-sequence authority was projected.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentMultiCollectionSequenceProjectionStatus
            .AdjacentMultiCollectionSequenceProjected;
}
