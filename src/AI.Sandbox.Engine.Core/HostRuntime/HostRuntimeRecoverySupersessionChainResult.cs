namespace AI.Sandbox.Engine.Core.HostRuntime;
/// <summary>
/// Represents one immutable Host recovery supersession-chain validation result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoverySupersessionChainResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoverySupersessionChainResult(
        HostRuntimeRecoverySupersessionChainStatus status,
        global::System.Collections.Generic.IReadOnlyList<
            HostRuntimeRecoveryCheckpointSupersession<
                TRequest,
                TState,
                TCompletion>> supersessions,
        HostRuntimeRecoverySupersessionChain<
            TRequest,
            TState,
            TCompletion>? chain)
    {
        Status = status;
        Supersessions = supersessions;
        Chain = chain;
    }

    /// <summary>Gets the explicit supersession-chain validation outcome.</summary>
    public HostRuntimeRecoverySupersessionChainStatus Status { get; }

    /// <summary>Gets the immutable supplied supersession snapshot.</summary>
    public global::System.Collections.Generic.IReadOnlyList<
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion>> Supersessions { get; }

    /// <summary>Gets validated supersession-chain authority when successful.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion>? Chain { get; }

    /// <summary>Gets whether supersession-chain authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoverySupersessionChainStatus.ChainValidated;
}
