namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery cross-collection checkpoint-range result.
/// </summary>
public sealed record HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCrossCollectionCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousCollectionPairStatus status,
        HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
            TRequest,
            TState,
            TCompletion> collectionPairSummary,
        HostRuntimeRecoveryCrossCollectionCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        CollectionPairSummary = collectionPairSummary;
        Query = query;
    }

    /// <summary>Gets the explicit cross-collection query outcome.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairStatus Status { get; }

    /// <summary>Gets unchanged source collection-pair-summary authority.</summary>
    public HostRuntimeRecoveryContinuousCollectionPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> CollectionPairSummary { get; }

    /// <summary>Gets the resolved cross-collection query, when successful.</summary>
    public HostRuntimeRecoveryCrossCollectionCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether cross-collection range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousCollectionPairStatus
            .CrossCollectionCheckpointRangeQueried;
}
