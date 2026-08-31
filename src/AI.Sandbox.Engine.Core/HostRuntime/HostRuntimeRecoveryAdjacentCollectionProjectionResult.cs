namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable adjacent-collection projection result.</summary>
public sealed record HostRuntimeRecoveryAdjacentCollectionProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryAdjacentCollectionProjectionResult(
        HostRuntimeRecoveryAdjacentCollectionProjectionStatus status,
        HostRuntimeRecoveryAdjacentCollectionSelection<
            TRequest,
            TState,
            TCompletion> selection,
        HostRuntimeRecoveryAdjacentCollectionProjection<
            TRequest,
            TState,
            TCompletion>? projection)
    {
        Status = status;
        Selection = selection;
        Projection = projection;
    }

    /// <summary>Gets explicit adjacent-collection projection outcome.</summary>
    public HostRuntimeRecoveryAdjacentCollectionProjectionStatus Status { get; }

    /// <summary>Gets unchanged adjacent-collection selection authority.</summary>
    public HostRuntimeRecoveryAdjacentCollectionSelection<
        TRequest,
        TState,
        TCompletion> Selection { get; }

    /// <summary>Gets projected adjacent-collection authority when successful.</summary>
    public HostRuntimeRecoveryAdjacentCollectionProjection<
        TRequest,
        TState,
        TCompletion>? Projection { get; }

    /// <summary>Gets whether adjacent-collection authority was projected.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryAdjacentCollectionProjectionStatus
            .AdjacentCollectionProjected;
}
