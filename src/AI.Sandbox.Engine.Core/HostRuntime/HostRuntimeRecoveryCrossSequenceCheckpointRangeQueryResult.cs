namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery cross-sequence checkpoint-range result.
/// </summary>
public sealed record HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCrossSequenceCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousMultiSequenceStatus status,
        HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
            TRequest,
            TState,
            TCompletion> multiSequenceSummary,
        HostRuntimeRecoveryCrossSequenceCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        MultiSequenceSummary = multiSequenceSummary;
        Query = query;
    }

    /// <summary>Gets the explicit cross-sequence query outcome.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceStatus Status { get; }

    /// <summary>Gets unchanged source multi-sequence-summary authority.</summary>
    public HostRuntimeRecoveryContinuousMultiSequenceSummaryProjection<
        TRequest,
        TState,
        TCompletion> MultiSequenceSummary { get; }

    /// <summary>Gets the resolved cross-sequence query, when successful.</summary>
    public HostRuntimeRecoveryCrossSequenceCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether cross-sequence range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousMultiSequenceStatus
            .CrossSequenceCheckpointRangeQueried;
}
