namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority for one acknowledged in-flight Host attempt.
/// </summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeInFlightAttempt<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeInFlightAttempt(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeAttemptIdKind> attemptId,
        HostRuntimeDispatchSelection<TRequest> selection,
        HostRuntimeRequestEnvelope<TRequest> request,
        HostRuntimeWorkLease<TRequest> lease,
        long acknowledgedTick)
    {
        AttemptId = attemptId;
        Selection = selection;
        Request = request;
        Lease = lease;
        AcknowledgedTick = acknowledgedTick;
        ObservedRequestRevision = request.Revision;
        ObservedLeaseRevision = lease.Revision;
    }

    /// <summary>Gets the externally assigned attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId { get; }

    /// <summary>
    /// Gets the unchanged dequeue and dispatch selection authority.
    /// </summary>
    public HostRuntimeDispatchSelection<TRequest> Selection { get; }

    /// <summary>
    /// Gets the current request authority accepted at acknowledgement.
    /// </summary>
    public HostRuntimeRequestEnvelope<TRequest> Request { get; }

    /// <summary>
    /// Gets the current active lease authority accepted at acknowledgement.
    /// </summary>
    public HostRuntimeWorkLease<TRequest> Lease { get; }

    /// <summary>
    /// Gets the external monotonic tick at acknowledgement.
    /// </summary>
    public long AcknowledgedTick { get; }

    /// <summary>
    /// Gets the request revision observed at acknowledgement.
    /// </summary>
    public long ObservedRequestRevision { get; }

    /// <summary>
    /// Gets the lease revision observed at acknowledgement.
    /// </summary>
    public long ObservedLeaseRevision { get; }

    /// <summary>Gets the unchanged acknowledged dispatch.</summary>
    public HostRuntimeDispatchEnvelope<TRequest> Dispatch =>
        Selection.Dispatch;

    /// <summary>Gets the stable queue ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId => Selection.QueueId;

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
        HostRuntimeDispatchIdKind> DispatchId =>
        Selection.Dispatch.DispatchId;

    /// <summary>Gets the acknowledged dispatch attempt number.</summary>
    public int AttemptNumber => Selection.Dispatch.AttemptNumber;

    /// <summary>Gets the matching monotonic clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Lease.ClockId;
}
