namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery cross-multi-collection-sequence checkpoint-range result.
/// </summary>
public sealed record HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus status,
        HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion> multiCollectionSequenceSequenceSequenceSummary,
        HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        MultiCollectionSequenceSequenceSequenceSummary = multiCollectionSequenceSequenceSequenceSummary;
        Query = query;
    }

    /// <summary>Gets the explicit cross-multi-collection query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus Status { get; }

    /// <summary>Gets unchanged source multi-collection-sequence-summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> MultiCollectionSequenceSequenceSequenceSummary { get; }

    /// <summary>Gets the resolved cross-multi-collection query, when successful.</summary>
    public HostRuntimeRecoveryCrossMultiCollectionSequenceSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether cross-multi-collection range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiCollectionSequenceSequenceSequenceSummaryStatus
            .CrossMultiCollectionSequenceSequenceCheckpointRangeQueried;
}
