namespace AI.Sandbox.Engine.Core.HostRuntime;
/// <summary>
/// Represents one immutable Host recovery latest-checkpoint selection result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryLatestCheckpointSelectionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryLatestCheckpointSelectionResult(
        HostRuntimeRecoverySupersessionChainStatus status,
        HostRuntimeRecoverySupersessionChain<
            TRequest,
            TState,
            TCompletion> chain,
        HostRuntimeRecoveryLatestCheckpointSelection<
            TRequest,
            TState,
            TCompletion>? selection)
    {
        Status = status;
        Chain = chain;
        Selection = selection;
    }

    /// <summary>Gets the explicit latest-checkpoint selection outcome.</summary>
    public HostRuntimeRecoverySupersessionChainStatus Status { get; }

    /// <summary>Gets unchanged validated supersession-chain authority.</summary>
    public HostRuntimeRecoverySupersessionChain<
        TRequest,
        TState,
        TCompletion> Chain { get; }

    /// <summary>Gets latest-checkpoint selection authority when successful.</summary>
    public HostRuntimeRecoveryLatestCheckpointSelection<
        TRequest,
        TState,
        TCompletion>? Selection { get; }

    /// <summary>Gets whether latest-checkpoint authority was selected.</summary>
    public bool Succeeded =>
        Status ==
        HostRuntimeRecoverySupersessionChainStatus.LatestCheckpointSelected;
}
