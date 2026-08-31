namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery cross-collection-sequence checkpoint-range result.
/// </summary>
public sealed record HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus status,
        HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
            TRequest,
            TState,
            TCompletion> multiCollectionSummary,
        HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        MultiCollectionSummary = multiCollectionSummary;
        Query = query;
    }

    /// <summary>Gets the explicit cross-collection-sequence query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus Status { get; }

    /// <summary>Gets unchanged source multi-collection-summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSummaryProjection<
        TRequest,
        TState,
        TCompletion> MultiCollectionSummary { get; }

    /// <summary>Gets the resolved cross-collection-sequence query, when successful.</summary>
    public HostRuntimeRecoveryCrossCollectionSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether cross-collection-sequence range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSummaryStatus
            .CrossCollectionSequenceCheckpointRangeQueried;
}
