namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority for one retry requeue and queue re-admission.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
/// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
public sealed record HostRuntimeRetryRequeue<TRequest, TCompletion>
    where TRequest : IHostRuntimeRequest
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRetryRequeue(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRetryRequeueIdKind> requeueId,
        HostRuntimeAttemptSettlement<TRequest, TCompletion> settlement,
        HostRuntimeRetryDecision<TRequest> retryDecision,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeQueueAdmission<TRequest> admission,
        long requeuedTick)
    {
        RequeueId = requeueId;
        Settlement = settlement;
        RetryDecision = retryDecision;
        Request = request;
        Admission = admission;
        RequeuedTick = requeuedTick;
        ObservedTerminalRequestRevision =
            settlement.ObservedRequestRevision;
        ObservedQueueRevision = admission.ObservedQueueRevision;
    }

    /// <summary>Gets the externally assigned retry requeue ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRetryRequeueIdKind> RequeueId { get; }

    /// <summary>Gets the unchanged terminal settlement authority.</summary>
    public HostRuntimeAttemptSettlement<TRequest, TCompletion>
        Settlement { get; }

    /// <summary>Gets the unchanged advisory retry decision.</summary>
    public HostRuntimeRetryDecision<TRequest> RetryDecision { get; }

    /// <summary>Gets the reopened pending request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets the resulting queue-admission authority.</summary>
    public HostRuntimeQueueAdmission<TRequest> Admission { get; }

    /// <summary>Gets the resulting immutable queue snapshot.</summary>
    public HostRuntimeQueueSnapshot Snapshot => Admission.Snapshot;

    /// <summary>Gets the deterministic immutable priority.</summary>
    public HostRuntimePriority Priority => Admission.Priority;

    /// <summary>Gets the external monotonic requeue tick.</summary>
    public long RequeuedTick { get; }

    /// <summary>Gets the terminal request revision observed before reopening.</summary>
    public long ObservedTerminalRequestRevision { get; }

    /// <summary>Gets the queue revision observed before re-admission.</summary>
    public long ObservedQueueRevision { get; }

    /// <summary>Gets the stable settlement ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeSettlementIdKind> SettlementId =>
        Settlement.SettlementId;

    /// <summary>Gets the stable completed attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId => Settlement.AttemptId;

    /// <summary>Gets the resulting admission ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAdmissionIdKind> AdmissionId =>
        Admission.AdmissionId;

    /// <summary>Gets the stable queue ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId => Admission.QueueId;

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Request.RequestId;

    /// <summary>Gets the completed attempt number.</summary>
    public int CompletedAttemptNumber =>
        RetryDecision.CompletedAttemptNumber;

    /// <summary>Gets the next one-based attempt number.</summary>
    public int NextAttemptNumber => RetryDecision.NextAttemptNumber;

    /// <summary>Gets the advisory retry tick.</summary>
    public long RetryAtTick => RetryDecision.RetryAtTick!.Value;

    /// <summary>Gets the matching monotonic clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => RetryDecision.ClockId;
}
