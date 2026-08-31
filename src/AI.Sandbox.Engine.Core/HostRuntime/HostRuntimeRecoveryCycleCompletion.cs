namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority closing one exact Host recovery cycle.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCycleCompletion<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCycleCompletion(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCycleCompletionIdKind> cycleCompletionId,
        HostRuntimeRecoveryResumedAttemptSettlement<
            TRequest,
            TState,
            TCompletion> resumedSettlement,
        long completedTick,
        long revision)
    {
        CycleCompletionId = cycleCompletionId;
        ResumedSettlement = resumedSettlement;
        CompletedTick = completedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned recovery-cycle completion ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCycleCompletionIdKind> CycleCompletionId { get; }

    /// <summary>Gets unchanged resumed-attempt settlement authority.</summary>
    public HostRuntimeRecoveryResumedAttemptSettlement<
        TRequest,
        TState,
        TCompletion> ResumedSettlement { get; }

    /// <summary>Gets the external monotonic recovery-cycle completion tick.</summary>
    public long CompletedTick { get; }

    /// <summary>Gets the recovery-cycle completion authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets unchanged resumed-attempt acknowledgement authority.</summary>
    public HostRuntimeRecoveryResumedAttemptAcknowledgement<TRequest, TState>
        Acknowledgement => ResumedSettlement.Acknowledgement;

    /// <summary>Gets unchanged recovery checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> Checkpoint =>
        ResumedSettlement.Checkpoint;

    /// <summary>Gets the terminal request authority.</summary>
    public HostRuntimeRequestEnvelope<TRequest> Request =>
        ResumedSettlement.Request;

    /// <summary>Gets the released recovery lease authority.</summary>
    public HostRuntimeWorkLease<TRequest> Lease => ResumedSettlement.Lease;

    /// <summary>Gets the immutable external completion report.</summary>
    public HostRuntimeCompletionEnvelope<TCompletion> ReportedCompletion =>
        ResumedSettlement.Completion;

    /// <summary>Gets the recovery settlement ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryResumedAttemptSettlementIdKind>
            RecoverySettlementId => ResumedSettlement.RecoverySettlementId;

    /// <summary>Gets the underlying terminal settlement ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeSettlementIdKind> SettlementId =>
        ResumedSettlement.SettlementId;

    /// <summary>Gets the resumed attempt ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId => ResumedSettlement.AttemptId;

    /// <summary>Gets the stable request ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId => ResumedSettlement.RequestId;

    /// <summary>Gets the released recovery lease ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeLeaseIdKind> LeaseId => ResumedSettlement.LeaseId;

    /// <summary>Gets the terminal completion kind.</summary>
    public HostRuntimeCompletionKind OutcomeKind =>
        ResumedSettlement.OutcomeKind;

    /// <summary>Gets the external monotonic resumed-attempt settlement tick.</summary>
    public long SettledTick => ResumedSettlement.SettledTick;
}
