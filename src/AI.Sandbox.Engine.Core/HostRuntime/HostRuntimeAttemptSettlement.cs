namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable terminal authority for one settled Host attempt.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
/// <typeparam name="TCompletion">Exact completion payload type.</typeparam>
public sealed record HostRuntimeAttemptSettlement<TRequest, TCompletion>
    where TRequest : IHostRuntimeRequest
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeAttemptSettlement(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeSettlementIdKind> settlementId,
        HostRuntimeInFlightAttempt<TRequest> attempt,
        HostRuntimeCompletionEnvelope<TCompletion> completion,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeWorkLease<TRequest> lease,
        long settledTick)
    {
        SettlementId = settlementId;
        Attempt = attempt;
        Completion = completion;
        Request = request;
        Lease = lease;
        SettledTick = settledTick;
        ObservedRequestRevision = request.Revision;
        ObservedLeaseRevision = lease.Revision;
    }

    /// <summary>Gets the externally assigned settlement ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeSettlementIdKind> SettlementId { get; }

    /// <summary>Gets the unchanged acknowledged attempt authority.</summary>
    public HostRuntimeInFlightAttempt<TRequest> Attempt { get; }

    /// <summary>Gets the matched immutable external completion.</summary>
    public HostRuntimeCompletionEnvelope<TCompletion> Completion { get; }

    /// <summary>Gets the resulting terminal request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets the resulting released lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease { get; }

    /// <summary>Gets the external monotonic settlement tick.</summary>
    public long SettledTick { get; }

    /// <summary>Gets the terminal request revision after settlement.</summary>
    public long ObservedRequestRevision { get; }

    /// <summary>Gets the released lease revision after settlement.</summary>
    public long ObservedLeaseRevision { get; }

    /// <summary>Gets the explicit terminal completion kind.</summary>
    public HostRuntimeCompletionKind OutcomeKind => Completion.Kind;

    /// <summary>Gets the stable attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId => Attempt.AttemptId;

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Request.RequestId;

    /// <summary>Gets the stable lease ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeLeaseIdKind> LeaseId => Lease.LeaseId;

    /// <summary>Gets the stable worker ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeWorkerIdKind> WorkerId => Lease.WorkerId;

    /// <summary>Gets the stable dispatch ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchIdKind> DispatchId => Completion.DispatchId;

    /// <summary>Gets the completed one-based attempt number.</summary>
    public int AttemptNumber => Completion.AttemptNumber;

    /// <summary>Gets the matching monotonic clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Lease.ClockId;
}
