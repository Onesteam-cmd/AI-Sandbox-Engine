namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery cross-multi-collection-sequence checkpoint-range result.
/// </summary>
public sealed record HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryStatus status,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion> multiCollectionSequenceSequenceSequenceSequenceSummary,
        HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        MultiCollectionSequenceSequenceSequenceSequenceSummary = multiCollectionSequenceSequenceSequenceSequenceSummary;
        Query = query;
    }

    /// <summary>Gets the explicit cross-multi-collection query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryStatus Status { get; }

    /// <summary>Gets unchanged source multi-collection-sequence-summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> MultiCollectionSequenceSequenceSequenceSequenceSummary { get; }

    /// <summary>Gets the resolved cross-multi-collection query, when successful.</summary>
    public HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether cross-multi-collection range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSequenceSummaryStatus
            .CrossMultiCollectionSequenceSequenceSequenceCheckpointRangeQueried;
}
