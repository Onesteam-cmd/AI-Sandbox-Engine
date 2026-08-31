namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Contains immutable bounded worker ownership of admitted Host work.</summary>
/// <typeparam name="TRequest">Exact request payload type.</typeparam>
public sealed record HostRuntimeWorkLease<TRequest>
    where TRequest : IHostRuntimeRequest
{
    internal HostRuntimeWorkLease(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeLeaseIdKind> leaseId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeWorkerIdKind> workerId,
        HostRuntimeQueueAdmission<TRequest> admission,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeClockIdKind> clockId,
        long acquiredTick,
        long expiresTick,
        HostRuntimeLeaseState state,
        long revision)
    {
        LeaseId = leaseId;
        WorkerId = workerId;
        Admission = admission;
        ClockId = clockId;
        AcquiredTick = acquiredTick;
        ExpiresTick = expiresTick;
        State = state;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned lease ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeLeaseIdKind> LeaseId { get; }

    /// <summary>Gets the externally assigned worker ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeWorkerIdKind> WorkerId { get; }

    /// <summary>Gets the immutable queue admission owned by this lease.</summary>
    public HostRuntimeQueueAdmission<TRequest> Admission { get; }

    /// <summary>Gets the externally owned monotonic clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeClockIdKind> ClockId { get; }

    /// <summary>Gets the external tick at which ownership was acquired.</summary>
    public long AcquiredTick { get; }

    /// <summary>Gets the exclusive external expiry tick.</summary>
    public long ExpiresTick { get; }

    /// <summary>Gets the immutable lease state.</summary>
    public HostRuntimeLeaseState State { get; }

    /// <summary>Gets the optimistic lease authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the stable admitted request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeRequestIdKind> RequestId => Admission.RequestId;

    /// <summary>Gets the stable queue ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<HostRuntimeQueueIdKind> QueueId => Admission.QueueId;

    /// <summary>Gets whether the named worker currently owns the admitted work.</summary>
    public bool IsActive => State == HostRuntimeLeaseState.Active;
}
