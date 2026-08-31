namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority reacquiring one new lease for re-admitted recovery work.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryLeaseReacquisition(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryLeaseReacquisitionIdKind> reacquisitionId,
        HostRuntimeRecoveryReadmission<TRequest, TState> readmission,
        HostRuntimeWorkLease<TRequest> lease,
        long reacquiredTick,
        long revision)
    {
        ReacquisitionId = reacquisitionId;
        Readmission = readmission;
        Lease = lease;
        ReacquiredTick = reacquiredTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned lease-reacquisition ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryLeaseReacquisitionIdKind> ReacquisitionId { get; }

    /// <summary>Gets unchanged recovery re-admission authority.</summary>
    public HostRuntimeRecoveryReadmission<TRequest, TState> Readmission { get; }

    /// <summary>Gets the new active lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease { get; }

    /// <summary>Gets the external monotonic lease-reacquisition tick.</summary>
    public long ReacquiredTick { get; }

    /// <summary>Gets the lease-reacquisition authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets unchanged resumed-work selection authority.</summary>
    public HostRuntimeResumedWorkSelection<TRequest, TState> Selection =>
        Readmission.Selection;

    /// <summary>Gets unchanged recovery queue admission authority.</summary>
    public HostRuntimeQueueAdmission<TRequest> Admission =>
        Readmission.Admission;

    /// <summary>Gets unchanged selected request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request => Lease.Admission.Request;

    /// <summary>Gets the stable selected attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId => Readmission.AttemptId;

    /// <summary>Gets the stable selected request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Lease.RequestId;

    /// <summary>Gets the prior checkpoint lease ID retained as evidence.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeLeaseIdKind> PriorLeaseId =>
        Selection.Candidate.LeaseId;

    /// <summary>Gets the new recovery lease ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeLeaseIdKind> LeaseId => Lease.LeaseId;

    /// <summary>Gets the worker owning the new recovery lease.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeWorkerIdKind> WorkerId => Lease.WorkerId;

    /// <summary>Gets the represented recovery queue ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId => Lease.QueueId;

    /// <summary>Gets the represented monotonic Host clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Lease.ClockId;
}
