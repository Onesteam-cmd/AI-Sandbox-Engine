namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery chain-summary projection result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryChainSummaryProjectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryChainSummaryProjectionResult(
        HostRuntimeRecoveryChainQueryStatus status,
        HostRuntimeRecoverySupersessionChain<
            TRequest,
            TState,
            TCompletion> chain,
        HostRuntimeRecoveryChainSummaryProjection<
            TRequest,
            TState,
            TCompletion>? projection)
    {
        Status = status;
        Chain = chain;
        Projection = projection;
    }

    /// <summary>Gets the explicit chain-summary projection outcome.</summary>
    public HostRuntimeRecoveryChainQueryStatus Status { get; }

    /// <summary>Gets unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain { get; }

    /// <summary>Gets chain-summary projection authority when successful.</summary>
    public HostRuntimeRecoveryChainSummaryProjection<
        TRequest,
        TState,
        TCompletion>? Projection { get; }

    /// <summary>Gets whether chain-summary projection authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoveryChainQueryStatus.ChainSummaryProjected;
}
