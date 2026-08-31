namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains a compact immutable projection of one completed and superseded Host
/// recovery cycle while retaining the exact supersession authority as evidence.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCompletedCycleSummary<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCompletedCycleSummary(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCompletedCycleSummaryIdKind> summaryId,
        HostRuntimeRecoveryCheckpointSupersession<
            TRequest,
            TState,
            TCompletion> supersession,
        long summarizedTick,
        long revision)
    {
        SummaryId = summaryId;
        Supersession = supersession;
        SummarizedTick = summarizedTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned completed-cycle summary ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCompletedCycleSummaryIdKind> SummaryId { get; }

    /// <summary>Gets unchanged checkpoint-supersession authority.</summary>
    public HostRuntimeRecoveryCheckpointSupersession<
        TRequest,
        TState,
        TCompletion> Supersession { get; }

    /// <summary>Gets the external monotonic summary tick.</summary>
    public long SummarizedTick { get; }

    /// <summary>Gets the completed-cycle summary authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the checkpoint-supersession identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointSupersessionIdKind> SupersessionId =>
        Supersession.SupersessionId;

    /// <summary>Gets the completed recovery-cycle identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCycleCompletionIdKind> CycleCompletionId =>
        Supersession.CycleCompletionId;

    /// <summary>Gets the superseded checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> PriorCheckpointId =>
        Supersession.PriorCheckpointId;

    /// <summary>Gets the successor checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> SuccessorCheckpointId =>
        Supersession.SuccessorCheckpointId;

    /// <summary>Gets the represented runtime-instance identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId =>
        Supersession.RuntimeInstanceId;

    /// <summary>Gets the represented deterministic composition identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCompositionIdKind> CompositionId =>
        Supersession.CompositionId;

    /// <summary>Gets the represented Host queue identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId => Supersession.QueueId;

    /// <summary>Gets the represented monotonic Host clock identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => Supersession.ClockId;

    /// <summary>Gets the represented World identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId =>
        Supersession.WorldId;

    /// <summary>Gets the stable request identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRequestIdKind> RequestId =>
        Supersession.CycleCompletion.RequestId;

    /// <summary>Gets the resumed attempt identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeAttemptIdKind> AttemptId =>
        Supersession.CycleCompletion.AttemptId;

    /// <summary>Gets the terminal completion kind.</summary>
    public HostRuntimeCompletionKind OutcomeKind =>
        Supersession.OutcomeKind;

    /// <summary>Gets the original checkpoint capture tick.</summary>
    public long PriorCheckpointCapturedTick =>
        Supersession.PriorCheckpoint.CapturedTick;

    /// <summary>Gets the completed recovery-cycle tick.</summary>
    public long CycleCompletedTick => Supersession.CycleCompletedTick;

    /// <summary>Gets the successor checkpoint capture tick.</summary>
    public long SuccessorCheckpointCapturedTick =>
        Supersession.SuccessorCapturedTick;

    /// <summary>Gets the checkpoint-supersession tick.</summary>
    public long SupersededTick => Supersession.SupersededTick;

    /// <summary>Gets the prior World State version.</summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        PriorWorldStateVersion =>
        Supersession.PriorCheckpoint.WorldSnapshotDocument.WorldStateVersion;

    /// <summary>Gets the successor World State version.</summary>
    public global::AI.Sandbox.Engine.Core.WorldState.WorldStateVersion
        SuccessorWorldStateVersion =>
        Supersession.SuccessorCheckpoint.WorldSnapshotDocument
            .WorldStateVersion;

    /// <summary>Gets the prior logical simulation tick.</summary>
    public ulong PriorSimulationTick =>
        Supersession.PriorCheckpoint.WorldSnapshotDocument.SimulationTick;

    /// <summary>Gets the successor logical simulation tick.</summary>
    public ulong SuccessorSimulationTick =>
        Supersession.SuccessorCheckpoint.WorldSnapshotDocument.SimulationTick;
}
