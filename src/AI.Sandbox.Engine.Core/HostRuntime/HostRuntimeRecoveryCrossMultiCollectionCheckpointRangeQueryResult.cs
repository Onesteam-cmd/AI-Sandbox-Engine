namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery cross-multi-collection checkpoint-range result.
/// </summary>
public sealed record HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus status,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion> multiCollectionSequenceSummary,
        HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        MultiCollectionSequenceSummary = multiCollectionSequenceSummary;
        Query = query;
    }

    /// <summary>Gets the explicit cross-multi-collection query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus Status { get; }

    /// <summary>Gets unchanged source multi-collection-sequence-summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> MultiCollectionSequenceSummary { get; }

    /// <summary>Gets the resolved cross-multi-collection query, when successful.</summary>
    public HostRuntimeRecoveryCrossMultiCollectionCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether cross-multi-collection range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSummaryStatus
            .CrossMultiCollectionCheckpointRangeQueried;
}
