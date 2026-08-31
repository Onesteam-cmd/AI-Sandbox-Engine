namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent multi-collection-sequence-sequence projection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionResult(
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus status,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection<
            TRequest,
            TState,
            TCompletion> selection,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<
            TRequest,
            TState,
            TCompletion>? projection)
    {
        Status = status;
        Selection = selection;
        Projection = projection;
    }

    /// <summary>Gets explicit adjacent multi-collection-sequence-sequence projection outcome.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus Status { get; }

    /// <summary>Gets unchanged adjacent multi-collection-sequence-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets projected adjacent multi-collection-sequence-sequence authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjection<
        TRequest,
        TState,
        TCompletion>? Projection { get; }

    /// <summary>Gets whether adjacent multi-collection-sequence-sequence authority was projected.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceProjectionStatus
            .AdjacentMultiCollectionSequenceSequenceProjected;
}
