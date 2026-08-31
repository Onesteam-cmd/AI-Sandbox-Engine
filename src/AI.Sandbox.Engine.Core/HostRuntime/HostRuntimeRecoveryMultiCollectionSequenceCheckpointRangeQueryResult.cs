namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-collection-sequence checkpoint-range query result.</summary>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus status,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
            TRequest,
            TState,
            TCompletion> sequence,
        HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        Sequence = sequence;
        Query = query;
    }

    /// <summary>Gets the explicit multi-collection-sequence query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus Status { get; }

    /// <summary>Gets unchanged source multi-collection-sequence validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceValidation<
        TRequest,
        TState,
        TCompletion> Sequence { get; }

    /// <summary>Gets the resolved multi-collection-sequence query, when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether multi-collection-sequence checkpoint-range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceStatus
            .MultiCollectionSequenceCheckpointRangeQueried;
}
