namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery lineage-window projection result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryLineageWindowProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryLineageWindowProjectionResult(
        HostRuntimeRecoveryLineageWindowStatus status,
        HostRuntimeRecoveryChainSummaryProjection<
            TRequest,
            TState,
            TCompletion> sourceProjection,
        HostRuntimeRecoveryLineageWindowProjection<
            TRequest,
            TState,
            TCompletion>? window)
    {
        Status = status;
        SourceProjection = sourceProjection;
        Window = window;
    }

    /// <summary>Gets the explicit lineage-window projection outcome.</summary>
    public HostRuntimeRecoveryLineageWindowStatus Status { get; }

    /// <summary>Gets unchanged source chain-summary projection authority.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion> SourceProjection { get; }

    /// <summary>Gets lineage-window projection authority when successful.</summary>
    public HostRuntimeRecoveryLineageWindowProjection<
        TRequest,
        TState,
        TCompletion>? Window { get; }

    /// <summary>Gets whether lineage-window authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryLineageWindowStatus
            .LineageWindowProjected;
}
