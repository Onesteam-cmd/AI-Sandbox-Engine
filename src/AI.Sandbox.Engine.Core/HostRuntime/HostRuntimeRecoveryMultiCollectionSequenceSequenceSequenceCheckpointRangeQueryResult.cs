namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-collection-sequence-sequence-sequence checkpoint-range query result.</summary>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus status,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation<
            TRequest,
            TState,
            TCompletion> sequence,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        Sequence = sequence;
        Query = query;
    }

    /// <summary>Gets the explicit multi-collection-sequence-sequence-sequence query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus Status { get; }

    /// <summary>Gets unchanged source multi-collection-sequence-sequence-sequence validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceValidation<
        TRequest,
        TState,
        TCompletion> Sequence { get; }

    /// <summary>Gets the resolved multi-collection-sequence-sequence-sequence query, when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether multi-collection-sequence-sequence-sequence checkpoint-range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceStatus
            .MultiCollectionSequenceSequenceSequenceCheckpointRangeQueried;
}
