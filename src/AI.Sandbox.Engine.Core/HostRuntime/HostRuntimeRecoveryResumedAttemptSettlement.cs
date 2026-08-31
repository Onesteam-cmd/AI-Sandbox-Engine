namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable recovery authority for one terminally settled resumed attempt.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryResumedAttemptSettlement<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryResumedAttemptSettlement(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryResumedAttemptSettlementIdKind>
                recoverySettlementId,
        HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>
            acknowledgement,
        HostRuntimeAttemptSettlement<TRequest, TCompletion> settlement,
        long revision)
    {
        RecoverySettlementId = recoverySettlementId;
        Acknowledgement = acknowledgement;
        Settlement = settlement;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned recovery settlement ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryResumedAttemptSettlementIdKind>
            RecoverySettlementId { get; }

    /// <summary>Gets unchanged resumed-attempt acknowledgement authority.</summary>
    public HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>
        Acknowledgement { get; }

    /// <summary>Gets the underlying terminal attempt-settlement authority.</summary>
    public HostRuntimeAttemptSettlement<TRequest, TCompletion> Settlement { get; }

    /// <summary>Gets the recovery settlement authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets unchanged recovery dispatch reconstruction authority.</summary>
    public HostRuntimeRecoveryDispatchReconstruction<TRequest, TState>
        Reconstruction => Acknowledgement.Reconstruction;

    /// <summary>Gets unchanged recovery lease-reacquisition authority.</summary>
    public HostRuntimeRecoveryLeaseReacquisition<TRequest, TState>
        Reacquisition => Acknowledgement.Reacquisition;

    /// <summary>Gets unchanged recovery re-admission authority.</summary>
    public HostRuntimeRecoveryReadmission<TRequest, TState> Readmission =>
        Reacquisition.Readmission;

    /// <summary>Gets unchanged resumed-work selection authority.</summary>
    public HostRuntimeResumedWorkSelection<TRequest, TState> Selection =>
        Readmission.Selection;

    /// <summary>Gets unchanged recovery resumption plan authority.</summary>
    public HostRuntimeRecoveryResumptionPlan<TRequest, TState> Plan =>
        Selection.Plan;

    /// <summary>Gets unchanged recovery continuation authority.</summary>
    public HostRuntimeRecoveryContinuation<TRequest, TState> Continuation =>
        Plan.Continuation;

    /// <summary>Gets unchanged recovery checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> Checkpoint =>
        Continuation.Checkpoint;

    /// <summary>Gets the settled resumed in-flight attempt.</summary>
    public HostRuntimeInFlightAttempt<TRequest> Attempt =>
        Acknowledgement.Attempt;

    /// <summary>Gets the prior checkpoint attempt retained as evidence.</summary>
    public HostRuntimeInFlightAttempt<TRequest> PriorAttempt =>
        Acknowledgement.PriorAttempt;

    /// <summary>Gets the resulting terminal request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request => Settlement.Request;

    /// <summary>Gets the resulting released recovery lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease => Settlement.Lease;

    /// <summary>Gets the matched immutable external completion.</summary>
    public HostRuntimeCompletionEnvelope<TCompletion> Completion =>
        Settlement.Completion;

    /// <summary>Gets the externally assigned underlying settlement ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeSettlementIdKind> SettlementId => Settlement.SettlementId;

    /// <summary>Gets the resumed-attempt acknowledgement ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryResumedAttemptAcknowledgementIdKind>
            AcknowledgementId => Acknowledgement.AcknowledgementId;

    /// <summary>Gets the resumed attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId => Settlement.AttemptId;

    /// <summary>Gets the prior checkpoint attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> PriorAttemptId =>
        Acknowledgement.PriorAttemptId;

    /// <summary>Gets the recovery dispatch ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeDispatchIdKind> DispatchId => Settlement.DispatchId;

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => Settlement.RequestId;

    /// <summary>Gets the released recovery lease ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeLeaseIdKind> LeaseId => Settlement.LeaseId;

    /// <summary>Gets the recovery worker ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeWorkerIdKind> WorkerId => Settlement.WorkerId;

    /// <summary>Gets the matching recovery clock domain.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Settlement.ClockId;

    /// <summary>Gets the settled resumed attempt number.</summary>
    public int AttemptNumber => Settlement.AttemptNumber;

    /// <summary>Gets the terminal completion kind.</summary>
    public HostRuntimeCompletionKind OutcomeKind => Settlement.OutcomeKind;

    /// <summary>Gets the external monotonic settlement tick.</summary>
    public long SettledTick => Settlement.SettledTick;
}
