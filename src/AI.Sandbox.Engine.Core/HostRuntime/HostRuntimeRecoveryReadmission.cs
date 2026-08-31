namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority re-admitting one selected recovery work item.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryReadmission<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryReadmission(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryReadmissionIdKind> readmissionId,
        HostRuntimeResumedWorkSelection<TRequest, TState> selection,
        HostRuntimeQueueAdmission<TRequest> admission,
        long readmittedTick,
        long revision)
    {
        ReadmissionId = readmissionId;
        Selection = selection;
        Admission = admission;
        ReadmittedTick = readmittedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned recovery re-admission ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryReadmissionIdKind> ReadmissionId { get; }

    /// <summary>Gets unchanged resumed-work selection authority.</summary>
    public HostRuntimeResumedWorkSelection<TRequest, TState> Selection { get; }

    /// <summary>Gets the new immutable queue admission authority.</summary>
    public HostRuntimeQueueAdmission<TRequest> Admission { get; }

    /// <summary>Gets the external monotonic re-admission tick.</summary>
    public long ReadmittedTick { get; }

    /// <summary>Gets the recovery re-admission authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets unchanged selected request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request => Admission.Request;

    /// <summary>Gets the stable selected attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId => Selection.AttemptId;

    /// <summary>Gets the stable selected request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Admission.RequestId;

    /// <summary>Gets the new recovery admission ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAdmissionIdKind> AdmissionId => Admission.AdmissionId;

    /// <summary>Gets the prior checkpoint admission ID retained as evidence.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAdmissionIdKind> PriorAdmissionId =>
        Selection.Candidate.Lease.Admission.AdmissionId;

    /// <summary>Gets the represented recovery queue ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId => Admission.QueueId;

    /// <summary>Gets the represented logical runtime instance ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId =>
        Selection.Plan.RuntimeInstanceId;

    /// <summary>Gets the represented monotonic Host clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Selection.Plan.ClockId;
}
