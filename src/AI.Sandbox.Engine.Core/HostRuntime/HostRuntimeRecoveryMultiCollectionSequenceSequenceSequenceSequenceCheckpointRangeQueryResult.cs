namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-collection-sequence-sequence-sequence-sequence checkpoint-range query result.</summary>
public sealed record HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus status,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation<
            TRequest,
            TState,
            TCompletion> sequence,
        HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        Sequence = sequence;
        Query = query;
    }

    /// <summary>Gets the explicit multi-collection-sequence-sequence-sequence-sequence query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus Status { get; }

    /// <summary>Gets unchanged source multi-collection-sequence-sequence-sequence-sequence validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceValidation<
        TRequest,
        TState,
        TCompletion> Sequence { get; }

    /// <summary>Gets the resolved multi-collection-sequence-sequence-sequence-sequence query, when successful.</summary>
    public HostRuntimeRecoveryMultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether multi-collection-sequence-sequence-sequence-sequence checkpoint-range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceStatus
            .MultiCollectionSequenceSequenceSequenceSequenceCheckpointRangeQueried;
}
