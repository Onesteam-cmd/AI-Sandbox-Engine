namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-collection-sequence-sequence checkpoint-range query result.</summary>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceStatus status,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidation<
            TRequest,
            TState,
            TCompletion> sequence,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        Sequence = sequence;
        Query = query;
    }

    /// <summary>Gets the explicit multi-collection-sequence-sequence query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceStatus Status { get; }

    /// <summary>Gets unchanged source multi-collection-sequence-sequence validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceValidation<
        TRequest,
        TState,
        TCompletion> Sequence { get; }

    /// <summary>Gets the resolved multi-collection-sequence-sequence query, when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether multi-collection-sequence-sequence checkpoint-range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceStatus
            .MultiCollectionSequenceSequenceCheckpointRangeQueried;
}
