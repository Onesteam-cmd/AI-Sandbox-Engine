namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-collection checkpoint-range query result.</summary>
public sealed record HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousCollectionSequenceStatus status,
        HostRuntimeRecoveryContinuousCollectionSequenceValidation<
            TRequest,
            TState,
            TCompletion> sequence,
        HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        Sequence = sequence;
        Query = query;
    }

    /// <summary>Gets the explicit multi-collection query outcome.</summary>
    public HostRuntimeRecoveryContinuousCollectionSequenceStatus Status { get; }

    /// <summary>Gets unchanged source collection-sequence authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionSequenceValidation<
        TRequest,
        TState,
        TCompletion> Sequence { get; }

    /// <summary>Gets the resolved multi-collection query, when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether multi-collection checkpoint-range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousCollectionSequenceStatus
            .MultiCollectionCheckpointRangeQueried;
}
