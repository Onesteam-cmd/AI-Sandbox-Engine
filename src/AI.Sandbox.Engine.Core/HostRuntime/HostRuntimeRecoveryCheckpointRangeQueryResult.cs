namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery checkpoint-range query result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCheckpointRangeQueryResult(
        HostRuntimeRecoveryLineageWindowStatus status,
        HostRuntimeRecoveryLineageWindowProjection<
            TRequest,
            TState,
            TCompletion> window,
        HostRuntimeRecoveryCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        Window = window;
        Query = query;
    }

    /// <summary>Gets the explicit checkpoint-range query outcome.</summary>
    public HostRuntimeRecoveryLineageWindowStatus Status { get; }

    /// <summary>Gets unchanged source lineage-window projection authority.</summary>
    public HostRuntimeRecoveryLineageWindowProjection<
        TRequest,
        TState,
        TCompletion> Window { get; }

    /// <summary>Gets checkpoint-range query authority when successful.</summary>
    public HostRuntimeRecoveryCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether checkpoint-range authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryLineageWindowStatus
            .CheckpointRangeResolved;
}
