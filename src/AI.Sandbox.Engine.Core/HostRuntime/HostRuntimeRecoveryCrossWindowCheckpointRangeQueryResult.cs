namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery cross-window checkpoint-range result.
/// </summary>
public sealed record HostRuntimeRecoveryCrossWindowCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCrossWindowCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousWindowPairStatus status,
        HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
            TRequest,
            TState,
            TCompletion> pairSummary,
        HostRuntimeRecoveryCrossWindowCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        PairSummary = pairSummary;
        Query = query;
    }

    /// <summary>Gets the explicit cross-window query outcome.</summary>
    public HostRuntimeRecoveryContinuousWindowPairStatus Status { get; }

    /// <summary>Gets unchanged source pair-summary authority.</summary>
    public HostRuntimeRecoveryContinuousWindowPairSummaryProjection<
        TRequest,
        TState,
        TCompletion> PairSummary { get; }

    /// <summary>Gets the resolved cross-window query, when successful.</summary>
    public HostRuntimeRecoveryCrossWindowCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether cross-window range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousWindowPairStatus
            .CrossWindowCheckpointRangeQueried;
}
