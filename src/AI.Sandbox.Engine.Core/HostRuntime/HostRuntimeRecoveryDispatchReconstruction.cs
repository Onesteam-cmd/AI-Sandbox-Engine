namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority reconstructing one recovery dispatch from
/// re-admitted and reacquired work.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
public sealed record HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
{
    internal HostRuntimeRecoveryDispatchReconstruction(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryDispatchReconstructionIdKind> reconstructionId,
        HostRuntimeRecoveryLeaseReacquisition<TRequest, TState> reacquisition,
        HostRuntimeDispatchSelection<TRequest> selection,
        long reconstructedTick,
        long revision)
    {
        ReconstructionId = reconstructionId;
        Reacquisition = reacquisition;
        Selection = selection;
        ReconstructedTick = reconstructedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned reconstruction ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryDispatchReconstructionIdKind> ReconstructionId { get; }

    /// <summary>Gets unchanged lease-reacquisition authority.</summary>
    public HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>
        Reacquisition { get; }

    /// <summary>Gets the new dequeue and dispatch-selection authority.</summary>
    public HostRuntimeDispatchSelection<TRequest> Selection { get; }

    /// <summary>Gets the external monotonic reconstruction tick.</summary>
    public long ReconstructedTick { get; }

    /// <summary>Gets the reconstruction authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the unchanged selected request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request => Reacquisition.Request;

    /// <summary>Gets the new active recovery lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease => Reacquisition.Lease;

    /// <summary>Gets the new immutable advisory recovery dispatch.</summary>
    public HostRuntimeDispatchEnvelope<TRequest> Dispatch => Selection.Dispatch;

    /// <summary>Gets the prior checkpoint attempt retained as evidence.</summary>
    public HostRuntimeInFlightAttempt<TRequest> PriorAttempt =>
        Reacquisition.Selection.Candidate.Attempt;

    /// <summary>Gets the prior checkpoint attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> PriorAttemptId => PriorAttempt.AttemptId;

    /// <summary>Gets the prior checkpoint dispatch-selection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchSelectionIdKind> PriorSelectionId =>
        PriorAttempt.Selection.SelectionId;

    /// <summary>Gets the new recovery dispatch-selection ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchSelectionIdKind> SelectionId => Selection.SelectionId;

    /// <summary>Gets the prior checkpoint dispatch ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchIdKind> PriorDispatchId => PriorAttempt.DispatchId;

    /// <summary>Gets the new recovery dispatch ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchIdKind> DispatchId => Dispatch.DispatchId;

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Request.RequestId;

    /// <summary>Gets the new recovery lease ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeLeaseIdKind> LeaseId => Lease.LeaseId;

    /// <summary>Gets the recovery worker ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeWorkerIdKind> WorkerId => Lease.WorkerId;

    /// <summary>Gets the represented recovery queue ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId => Selection.QueueId;

    /// <summary>Gets the represented monotonic Host clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Lease.ClockId;

    /// <summary>Gets the resumed dispatch attempt number.</summary>
    public int AttemptNumber => Dispatch.AttemptNumber;
}
