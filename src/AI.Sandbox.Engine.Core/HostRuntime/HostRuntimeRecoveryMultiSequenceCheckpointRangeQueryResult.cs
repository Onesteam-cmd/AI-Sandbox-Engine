namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-sequence checkpoint-range query result.</summary>
public sealed record HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiSequenceCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus status,
        HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
            TRequest,
            TState,
            TCompletion> collection,
        HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        Collection = collection;
        Query = query;
    }

    /// <summary>Gets the explicit multi-sequence query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus Status { get; }

    /// <summary>Gets unchanged source collection-validation authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceCollectionValidation<
        TRequest,
        TState,
        TCompletion> Collection { get; }

    /// <summary>Gets the resolved multi-sequence query, when successful.</summary>
    public HostRuntimeRecoveryMultiSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether multi-sequence checkpoint-range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiSequenceCollectionStatus
            .MultiSequenceCheckpointRangeQueried;
}
