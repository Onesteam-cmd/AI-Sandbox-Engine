namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery checkpoint-lineage query result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpointLineageQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCheckpointLineageQueryResult(
        HostRuntimeRecoveryChainQueryStatus status,
        HostRuntimeRecoveryChainSummaryProjection<
            TRequest,
            TState,
            TCompletion> projection,
        HostRuntimeRecoveryCheckpointLineageQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        Projection = projection;
        Query = query;
    }

    /// <summary>Gets the explicit checkpoint-lineage query outcome.</summary>
    public HostRuntimeRecoveryChainQueryStatus Status { get; }

    /// <summary>Gets unchanged chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> Projection { get; }

    /// <summary>Gets checkpoint-lineage query authority when successful.</summary>
    public HostRuntimeRecoveryCheckpointLineageQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether checkpoint lineage was resolved.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryChainQueryStatus.CheckpointLineageResolved;
}
