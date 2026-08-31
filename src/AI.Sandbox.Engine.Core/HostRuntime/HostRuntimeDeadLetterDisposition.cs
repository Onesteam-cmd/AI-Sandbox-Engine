namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority for one terminal Host dead-letter disposition.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
/// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
public sealed record HostRuntimeDeadLetterDisposition<TRequest, TCompletion>
    where TRequest : IHostRuntimeRequest
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeDeadLetterDisposition(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeDeadLetterDispositionIdKind> dispositionId,
        HostRuntimeDeadLetterDispositionKind kind,
        HostRuntimeAttemptSettlement<TRequest, TCompletion> settlement,
        HostRuntimeRetryDecision<TRequest> retryDecision,
        long disposedTick)
    {
        DispositionId = dispositionId;
        Kind = kind;
        Settlement = settlement;
        RetryDecision = retryDecision;
        DisposedTick = disposedTick;
        ObservedTerminalRequestRevision =
            settlement.ObservedRequestRevision;
    }

    /// <summary>Gets the externally assigned disposition ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDeadLetterDispositionIdKind> DispositionId { get; }

    /// <summary>Gets the exact terminal disposition kind.</summary>
    public HostRuntimeDeadLetterDispositionKind Kind { get; }

    /// <summary>Gets the unchanged terminal settlement authority.</summary>
    public HostRuntimeAttemptSettlement<TRequest, TCompletion>
        Settlement { get; }

    /// <summary>Gets the unchanged denied retry decision.</summary>
    public HostRuntimeRetryDecision<TRequest> RetryDecision { get; }

    /// <summary>Gets the external monotonic disposition tick.</summary>
    public long DisposedTick { get; }

    /// <summary>Gets the terminal request revision observed at disposition.</summary>
    public long ObservedTerminalRequestRevision { get; }

    /// <summary>Gets the unchanged terminal request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request =>
        Settlement.Request;

    /// <summary>Gets the unchanged terminal completion authority.</summary>
    public HostRuntimeCompletionEnvelope<TCompletion> Completion =>
        Settlement.Completion;

    /// <summary>Gets the terminal completion kind.</summary>
    public HostRuntimeCompletionKind OutcomeKind =>
        Settlement.OutcomeKind;

    /// <summary>Gets the terminal retry-decision status.</summary>
    public HostRuntimeRetryDecisionStatus RetryDecisionStatus =>
        RetryDecision.Status;

    /// <summary>Gets the unchanged retry policy.</summary>
    public HostRuntimeRetryPolicy Policy => RetryDecision.Policy;

    /// <summary>Gets the optional unchanged external deadline.</summary>
    public HostRuntimeDeadline? Deadline => RetryDecision.Deadline;

    /// <summary>Gets the exact unchanged retry reason.</summary>
    public IHostRuntimeRetryReason Reason => RetryDecision.Reason;

    /// <summary>Gets the stable settlement ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeSettlementIdKind> SettlementId =>
        Settlement.SettlementId;

    /// <summary>Gets the stable completed attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId => Settlement.AttemptId;

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Request.RequestId;

    /// <summary>Gets the stable worker ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeWorkerIdKind> WorkerId => Settlement.WorkerId;

    /// <summary>Gets the stable dispatch ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchIdKind> DispatchId =>
        Settlement.DispatchId;

    /// <summary>Gets the terminal one-based attempt number.</summary>
    public int AttemptNumber => Settlement.AttemptNumber;

    /// <summary>Gets the matching monotonic clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Settlement.ClockId;
}
