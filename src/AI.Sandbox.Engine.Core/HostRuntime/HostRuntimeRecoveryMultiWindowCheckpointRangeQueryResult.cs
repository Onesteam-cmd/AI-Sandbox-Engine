namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable multi-window checkpoint-range query result.</summary>
public sealed record HostRuntimeRecoveryMultiWindowCheckpointRangeQueryResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryMultiWindowCheckpointRangeQueryResult(
        HostRuntimeRecoveryContinuousWindowSequenceStatus status,
        HostRuntimeRecoveryContinuousWindowSequenceValidation<
            TRequest,
            TState,
            TCompletion> sequence,
        HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
            TRequest,
            TState,
            TCompletion>? query)
    {
        Status = status;
        Sequence = sequence;
        Query = query;
    }

    /// <summary>Gets the explicit multi-window query outcome.</summary>
    public HostRuntimeRecoveryContinuousWindowSequenceStatus Status { get; }

    /// <summary>Gets unchanged source sequence-validation authority.</summary>
    public HostRuntimeRecoveryContinuousWindowSequenceValidation<
        TRequest,
        TState,
        TCompletion> Sequence { get; }

    /// <summary>Gets the resolved multi-window query, when successful.</summary>
    public HostRuntimeRecoveryMultiWindowCheckpointRangeQuery<
        TRequest,
        TState,
        TCompletion>? Query { get; }

    /// <summary>Gets whether multi-window checkpoint-range query succeeded.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryContinuousWindowSequenceStatus
            .MultiWindowCheckpointRangeQueried;
}
