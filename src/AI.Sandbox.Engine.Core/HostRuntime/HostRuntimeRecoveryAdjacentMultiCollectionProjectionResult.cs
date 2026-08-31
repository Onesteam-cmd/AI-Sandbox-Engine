namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent multi-collection projection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentMultiCollectionProjectionResult(
        HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus status,
        HostRuntimeRecoveryAdjacentMultiCollectionSelection<
            TRequest,
            TState,
            TCompletion> selection,
        HostRuntimeRecoveryAdjacentMultiCollectionProjection<
            TRequest,
            TState,
            TCompletion>? projection)
    {
        Status = status;
        Selection = selection;
        Projection = projection;
    }

    /// <summary>Gets explicit adjacent multi-collection projection outcome.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus Status { get; }

    /// <summary>Gets unchanged adjacent multi-collection selection authority.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets projected adjacent multi-collection authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentMultiCollectionProjection<
        TRequest,
        TState,
        TCompletion>? Projection { get; }

    /// <summary>Gets whether adjacent multi-collection authority was projected.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentMultiCollectionProjectionStatus
            .AdjacentMultiCollectionProjected;
}
