namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Represents one immutable Host recovery resumed-attempt settlement result.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryResumedAttemptSettlementResult<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryResumedAttemptSettlementResult(
        HostRuntimeRecoverySettlementStatus status,
        HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>
            acknowledgement,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeWorkLease<TRequest> lease,
        HostRuntimeCompletionEnvelope<TCompletion> completion,
        HostRuntimeAttemptSettlementStatus? attemptSettlementStatus,
        HostRuntimeRecoveryResumedAttemptSettlement<
            TRequest,
            TState,
            TCompletion>? settlement)
    {
        Status = status;
        Acknowledgement = acknowledgement;
        Request = request;
        Lease = lease;
        Completion = completion;
        AttemptSettlementStatus = attemptSettlementStatus;
        Settlement = settlement;
    }

    /// <summary>Gets the explicit recovery settlement outcome.</summary>
    public HostRuntimeRecoverySettlementStatus Status { get; }

    /// <summary>Gets unchanged resumed-attempt acknowledgement authority.</summary>
    public HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>
        Acknowledgement { get; }

    /// <summary>Gets resulting or unchanged request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets resulting or unchanged recovery lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease { get; }

    /// <summary>Gets the unchanged external completion report.</summary>
    public HostRuntimeCompletionEnvelope<TCompletion> Completion { get; }

    /// <summary>Gets the underlying attempt-settlement outcome when invoked.</summary>
    public HostRuntimeAttemptSettlementStatus? AttemptSettlementStatus { get; }

    /// <summary>Gets recovery settlement authority when successful.</summary>
    public HostRuntimeRecoveryResumedAttemptSettlement<
        TRequest,
        TState,
        TCompletion>? Settlement { get; }

    /// <summary>Gets whether terminal recovery settlement authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeRecoverySettlementStatus.AttemptSettled;
}
