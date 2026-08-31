namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery-cycle completion result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCycleCompletionResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCycleCompletionResult(
        HostRuntimeRecoverySettlementStatus status,
        HostRuntimeRecoveryResumedAttemptSettlement<
            TRequest,
            TState,
            TCompletion> settlement,
        HostRuntimeRecoveryCycleCompletion<
            TRequest,
            TState,
            TCompletion>? cycleCompletion)
    {
        Status = status;
        Settlement = settlement;
        CycleCompletion = cycleCompletion;
    }

    /// <summary>Gets the explicit recovery-cycle outcome.</summary>
    public HostRuntimeRecoverySettlementStatus Status { get; }

    /// <summary>Gets unchanged resumed-attempt settlement authority.</summary>
    public HostRuntimeRecoveryResumedAttemptSettlement<
        TRequest,
        TState,
        TCompletion> Settlement { get; }

    /// <summary>Gets recovery-cycle completion authority when successful.</summary>
    public HostRuntimeRecoveryCycleCompletion<
        TRequest,
        TState,
        TCompletion>? CycleCompletion { get; }

    /// <summary>Gets whether the exact recovery cycle was closed.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoverySettlementStatus.CycleCompleted;
}
