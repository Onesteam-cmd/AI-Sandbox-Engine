namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority for one abandoned acknowledged Host attempt.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeAbandonedAttemptDisposition<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeAbandonedAttemptDisposition(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAbandonedAttemptDispositionIdKind> dispositionId,
        HostRuntimeAbandonedAttemptDispositionKind kind,
        HostRuntimeInFlightAttempt<TRequest> attempt,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeWorkLease<TRequest> lease,
        long disposedTick)
    {
        DispositionId = dispositionId;
        Kind = kind;
        Attempt = attempt;
        Request = request;
        Lease = lease;
        DisposedTick = disposedTick;
        ObservedRequestRevision = request.Revision;
        ObservedLeaseRevision = lease.Revision;
    }

    /// <summary>Gets the externally assigned disposition ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAbandonedAttemptDispositionIdKind> DispositionId { get; }

    /// <summary>Gets the exact abandonment reason.</summary>
    public HostRuntimeAbandonedAttemptDispositionKind Kind { get; }

    /// <summary>Gets the unchanged acknowledged attempt authority.</summary>
    public HostRuntimeInFlightAttempt<TRequest> Attempt { get; }

    /// <summary>Gets the resulting terminal request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>Gets the resulting released or expired lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease { get; }

    /// <summary>Gets the external monotonic disposition tick.</summary>
    public long DisposedTick { get; }

    /// <summary>Gets the terminal request revision after disposition.</summary>
    public long ObservedRequestRevision { get; }

    /// <summary>Gets the released or expired lease revision after disposition.</summary>
    public long ObservedLeaseRevision { get; }

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
        HostRuntimeDispatchIdKind> DispatchId => Attempt.DispatchId;

    /// <summary>Gets the abandoned one-based attempt number.</summary>
    public int AttemptNumber => Attempt.AttemptNumber;

    /// <summary>Gets the matching monotonic clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Lease.ClockId;

    /// <summary>Gets the recorded cancellation reason, when present.</summary>
    public IHostRuntimeCancellationReason? CancellationReason =>
        Request.CancellationReason;
}
