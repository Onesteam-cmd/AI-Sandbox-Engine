namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one immutable Host attempt-settlement result.</summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
/// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
public sealed record HostRuntimeAttemptSettlementResult<
    TRequest,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeAttemptSettlementResult(
        HostRuntimeAttemptSettlementStatus status,
        HostRuntimeAttemptSettlement<TRequest, TCompletion>? settlement,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeWorkLease<TRequest> lease,
        HostRuntimeCompletionEnvelope<TCompletion> completion)
    {
        Status = status;
        Settlement = settlement;
        Request = request;
        Lease = lease;
        Completion = completion;
    }

    /// <summary>Gets the explicit settlement outcome.</summary>
    public HostRuntimeAttemptSettlementStatus Status { get; }

    /// <summary>
    /// Gets terminal settlement authority when settlement succeeded.
    /// </summary>
    public HostRuntimeAttemptSettlement<TRequest, TCompletion>?
        Settlement { get; }

    /// <summary>Gets resulting or unchanged request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets resulting or unchanged lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease { get; }

    /// <summary>Gets the unchanged reported completion.</summary>
    public HostRuntimeCompletionEnvelope<TCompletion> Completion { get; }

    /// <summary>Gets whether terminal settlement authority was created.</summary>
    public bool Succeeded =>
        Status == HostRuntimeAttemptSettlementStatus.Settled;
}
