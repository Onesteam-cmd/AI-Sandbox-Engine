namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent multi-collection-sequence-sequence-sequence projection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionResult(
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus status,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<
            TRequest,
            TState,
            TCompletion> selection,
        HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<
            TRequest,
            TState,
            TCompletion>? projection)
    {
        Status = status;
        Selection = selection;
        Projection = projection;
    }

    /// <summary>Gets explicit adjacent multi-collection-sequence-sequence-sequence projection outcome.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus Status { get; }

    /// <summary>Gets unchanged adjacent multi-collection-sequence-sequence-sequence selection authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets projected adjacent multi-collection-sequence-sequence-sequence authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjection<
        TRequest,
        TState,
        TCompletion>? Projection { get; }

    /// <summary>Gets whether adjacent multi-collection-sequence-sequence-sequence authority was projected.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentMultiCollectionSequenceSequenceSequenceProjectionStatus
            .AdjacentMultiCollectionSequenceSequenceSequenceProjected;
}
