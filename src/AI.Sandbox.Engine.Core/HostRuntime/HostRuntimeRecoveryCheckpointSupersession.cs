namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Contains immutable authority linking one completed recovery cycle to one
/// successor recovery checkpoint without deleting either authority.
/// </summary>
/// <typeparam name="TRequest">Exact Host request payload type.</typeparam>
/// <typeparam name="TState">Exact immutable World State root type.</typeparam>
/// <typeparam name="TCompletion">Exact Host completion payload type.</typeparam>
public sealed record HostRuntimeRecoveryCheckpointSupersession<
    TRequest,
    TState,
    TCompletion>
    where TRequest : IHostRuntimeRequest
    where TState : class, global::AI.Sandbox.Engine.Core.WorldState.IWorldState
    where TCompletion : IHostRuntimeCompletion
{
    internal HostRuntimeRecoveryCheckpointSupersession(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeRecoveryCheckpointSupersessionIdKind> supersessionId,
        HostRuntimeRecoveryCycleCompletion<TRequest, TState, TCompletion>
            cycleCompletion,
        HostRuntimeRecoveryCheckpoint<TRequest> successorCheckpoint,
        long supersededTick,
        long revision)
    {
        SupersessionId = supersessionId;
        CycleCompletion = cycleCompletion;
        SuccessorCheckpoint = successorCheckpoint;
        SupersededTick = supersededTick;
        Revision = revision;
    }

    /// <summary>Gets the externally assigned checkpoint-supersession ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointSupersessionIdKind> SupersessionId { get; }

    /// <summary>Gets unchanged completed recovery-cycle authority.</summary>
    public HostRuntimeRecoveryCycleCompletion<TRequest, TState, TCompletion>
        CycleCompletion { get; }

    /// <summary>Gets unchanged successor recovery-checkpoint authority.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> SuccessorCheckpoint { get; }

    /// <summary>Gets the external monotonic checkpoint-supersession tick.</summary>
    public long SupersededTick { get; }

    /// <summary>Gets the checkpoint-supersession authority revision.</summary>
    public long Revision { get; }

    /// <summary>Gets the prior checkpoint retained as immutable evidence.</summary>
    public HostRuntimeRecoveryCheckpoint<TRequest> PriorCheckpoint =>
        CycleCompletion.Checkpoint;

    /// <summary>Gets the completed recovery-cycle identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCycleCompletionIdKind> CycleCompletionId =>
        CycleCompletion.CycleCompletionId;

    /// <summary>Gets the superseded checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> PriorCheckpointId =>
        PriorCheckpoint.CheckpointId;

    /// <summary>Gets the successor checkpoint identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeRecoveryCheckpointIdKind> SuccessorCheckpointId =>
        SuccessorCheckpoint.CheckpointId;

    /// <summary>Gets the represented runtime-instance identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeInstanceIdKind> RuntimeInstanceId =>
        SuccessorCheckpoint.RuntimeInstanceId;

    /// <summary>Gets the represented deterministic composition identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCompositionIdKind> CompositionId =>
        SuccessorCheckpoint.Composition.CompositionId;

    /// <summary>Gets the represented Host queue identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeQueueIdKind> QueueId =>
        SuccessorCheckpoint.QueueSnapshot.QueueId;

    /// <summary>Gets the represented monotonic Host clock identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeClockIdKind> ClockId => SuccessorCheckpoint.ClockId;

    /// <summary>Gets the represented World identity.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId =>
        SuccessorCheckpoint.WorldSnapshotDocument.WorldId;

    /// <summary>Gets the terminal recovery outcome.</summary>
    public HostRuntimeCompletionKind OutcomeKind =>
        CycleCompletion.OutcomeKind;

    /// <summary>Gets the completed recovery-cycle tick.</summary>
    public long CycleCompletedTick => CycleCompletion.CompletedTick;

    /// <summary>Gets the successor checkpoint capture tick.</summary>
    public long SuccessorCapturedTick => SuccessorCheckpoint.CapturedTick;
}
