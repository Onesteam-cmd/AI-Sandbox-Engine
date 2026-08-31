namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery cross-multi-collection-sequence checkpoint-range result.
/// </summary>
public sealed record HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus status,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion> multiCollectionSequenceSequenceSummary,
        HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        MultiCollectionSequenceSequenceSummary = multiCollectionSequenceSequenceSummary;
        Query = query;
    }

    /// <summary>Gets the explicit cross-multi-collection query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus Status { get; }

    /// <summary>Gets unchanged source multi-collection-sequence-summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> MultiCollectionSequenceSequenceSummary { get; }

    /// <summary>Gets the resolved cross-multi-collection query, when successful.</summary>
    public HostRuntimeRecoveryCrossMultiCollectionSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether cross-multi-collection range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSummaryStatus
            .CrossMultiCollectionSequenceCheckpointRangeQueried;
}
